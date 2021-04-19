using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chigiri.DietShaper.Editor
{
    public class NearestPointResolver
    {

        interface BoneGroup
        {
            // Returns nearestPoint, timeInCurve, distance
            (Vector3, float, float) NearestPoint(Vector3 vertex);
        }

        public class BoneGroup2 : BoneGroup
        {

            Vector3 start;
            Vector3 v;
            Vector3 vIdent;
            float vLength;
            bool isLeaf;

            public BoneGroup2(Vector3 start, Vector3 end, bool isLeaf)
            {
                this.start = start;
                v = end - start;
                vIdent = v.normalized;
                vLength = v.magnitude;
                this.isLeaf = isLeaf;
            }

            public (Vector3, float, float) NearestPoint(Vector3 vertex)
            {
                // 頂点からボーン線分に下ろした垂線の足を求める
                var t = Vector3.Dot(vIdent, vertex - start) / vLength;
                if (t < 0f || 1f < t && !isLeaf) return (vertex, 0f, Mathf.Infinity); // 垂線の足がボーン線分上になければ変形しない
                var nearest = start + v * t;
                var distance = (vertex - nearest).magnitude;
                if (isLeaf) nearest = start;
                return (nearest, t, distance);
            }

        }

        // https://github.com/chigirits/DietShaper/issues/1
        public class BoneGroup3 : BoneGroup
        {

            Vector3 pB0;
            Vector3 pB1;
            Vector3 pB2;
            Vector3 vB1B0;
            Vector3 vB1B2;
            Vector3 N;
            Vector3 vX;
            Vector3 vY;
            Vector2 qB0;
            Vector2 qB2;
            Vector2 qC;
            BoneGroup2 altForLinear;

            public BoneGroup3(Vector3 pB0, Vector3 pB1, Vector3 pB2)
            {
                this.pB0 = pB0;
                this.pB1 = pB1;
                this.pB2 = pB2;

                // 1. 平面 B₀B₁B₂ の法線 N を、外積 B₁B₀×B₁B₂ から求める。
                vB1B0 = pB0 - pB1;
                vB1B2 = pB2 - pB1;
                N = Vector3.Cross(vB1B0, vB1B2).normalized; // Issueの図では手前向き（左手座標系）

                // 2. 平面上の座標空間を以下のように定義する。
                //    - B₁ を原点 O とする
                //    - B₁B₀ の単位ベクトルを取り、その方向を𝓍軸とする
                //    - 𝓍軸を N の周りに反時計回り90度回転した方向を𝓎軸とする
                vX = vB1B0.normalized;
                vY = Vector3.Cross(vX, N).normalized;

                // 4. 平面上の𝓍・𝓎軸単位ベクトルとの内積から、平面上での B₀, B₂, Pₚ の座標を求める。
                qB0 = new Vector2(Vector3.Dot(vB1B0, vX), Vector3.Dot(vB1B0, vY));
                qB2 = new Vector2(Vector3.Dot(vB1B2, vX), Vector3.Dot(vB1B2, vY));

                if (Mathf.Abs(qB2.y) < 1e-5f)
                {
                    // 3ボーンが一直線上に並ぶときは2ボーン処理で代替
                    altForLinear = new BoneGroup2(pB0, pB2, false);
                    return;
                }

                // 5. 次の2直線の交点を C とする。
                //    - B₀ を通り B₀O と垂直な直線：𝓍 = B₀x
                //    - B₂ を通り B₂O と垂直な直線：𝓎-B₂y = (-B₂x/B₂y)(𝓍-B₂x)
                // 6. 上記2直線の式を連立方程式として解き、C の座標を求める。
                //    - Cx = B₀x
                //    - Cy = (-B₂x/B₂y)(B₀x-B₂x) + B₂y
                qC = new Vector2(qB0.x, -qB2.x * (qB0.x-qB2.x) / qB2.y + qB2.y);
            }

            public (Vector3, float, float) NearestPoint(Vector3 pP)
            {
                if (altForLinear != null) return altForLinear.NearestPoint(pP);

                // 3. P から平面への垂線の足を Pₚ とする（Vector3.ProjectOnPlane を用いて B₁P を投影する）。
                // 4. 平面上の𝓍・𝓎軸単位ベクトルとの内積から、平面上での B₀, B₂, Pₚ の座標を求める。
                var vB1P = pP - pB1;
                var qPp = new Vector2(Vector3.Dot(vB1P, vX), Vector3.Dot(vB1P, vY));

                // 7. 外積 CO×CPₚ の符号から、Pₚ が OC を隔てて B₀, B₂ のどちら側に属するかを判定する（属する方のボーンを B とする）。
                var wCO = -qC;
                var wCPp = qPp - qC;
                var s = Mathf.Sign(Helper.Vector2Cross(wCO, wCPp));
                var qB = s < 0 ? qB0 : qB2;
                var pB = s < 0 ? pB0 : pB2;

                // 8. Pₚ から CB への垂線の足 F を求める（CB の単位ベクトルを I とすると、CF = CPₚ・I）。
                var dCB = (qB - qC).magnitude;
                var wI = (qB - qC).normalized;
                var dCF = Vector2.Dot(wCPp, wI);
                var wCF = wI * dCF;

                // 9. FPₚ と CO の交点 G を求める（**CG** = (CF/CB)**CO**）。
                var wCG = wCO * (dCF/dCB);

                // 10. r = GPₚ / GF = △CGPₚ / △CGF = |**CG**×**CPₚ**| / |**CG**×**CF**| を求める。
                var r = Helper.Vector2Cross(wCG, wCPp) / Helper.Vector2Cross(wCG, wCF);
                if (r < 0f || 1f < r) return (pP, 0f, Mathf.Infinity); // 垂線の足がボーン線分上になければ変形しない

                // 11. B₁B の r : 1-r の内分点として P' を求める。
                var pPr = Vector3.Lerp(pB1, pB, r);

                var t = (1f + s*r) * 0.5f;
                var d = (pPr - pP).magnitude;
                return (pPr, t, d);
            }

        }

        Animator avatarRoot;
        ShapeKey key;
        BoneGroup[] groups;

        public NearestPointResolver(Animator avatarRoot, ShapeKey key, bool isGenericMode)
        {
            this.avatarRoot = avatarRoot;
            this.key = key;
            groups = new BoneGroup[key.bodyLines.Count];
            for (var i = 0; i < key.bodyLines.Count; i++)
            {
                var b = key.bodyLines[i];
                switch (b.bones.Count)
                {
                    case 2:
                    {
                        var b0 = b.GetBoneTransform(0, avatarRoot, isGenericMode).position;
                        var b1 = b.GetBoneTransform(1, avatarRoot, isGenericMode).position;
                        groups[i] = new BoneGroup2(
                            Vector3.Lerp(b0, b1, key.startMargin),
                            Vector3.Lerp(b0, b1, 1f - key.endMargin),
                            key.isLeaf
                        );
                        break;
                    }
                    case 3:
                    {
                        var b0 = b.GetBoneTransform(0, avatarRoot, isGenericMode).position;
                        var b1 = b.GetBoneTransform(1, avatarRoot, isGenericMode).position;
                        var b2 = b.GetBoneTransform(2, avatarRoot, isGenericMode).position;
                        groups[i] = new BoneGroup3(
                            Vector3.Lerp(b0, b1, key.startMargin * 2f),
                            b1,
                            Vector3.Lerp(b1, b2, 1f - key.endMargin * 2f)
                        );
                        break;
                    }
                    default:
                        Debug.Log("Body line must have 2 or 3 bones");
                        break;
                }
            }
        }

        /// 指定された点から最も近いボーン線分への垂線の足を求めます。
        /// <param name="v">対象とする点のグローバル座標</param>
        /// <returns>垂線の足のグローバル座標、折れ線上の位置(0…1)、グローバル距離</returns>
        public (Vector3, float, float) Resolve(Vector3 v)
        {
            var va = avatarRoot.transform.InverseTransformPoint(v);
            var distance = Mathf.Infinity;
            var result = v;
            var time = 0f;
            for (var i = 0; i < key.bodyLines.Count; i++)
            {
                var bodyLine = key.bodyLines[i];
                var (p, t, d) = groups[i].NearestPoint(v);
                if (distance <= d) continue; // 最も近いbodyLineだけを採用
                if (bodyLine.xSignRange == SignRange.positive && va.x < 0f) continue;
                if (bodyLine.xSignRange == SignRange.negative && 0f < va.x) continue;
                result = p;
                time = t;
                distance = d;
            }
            var r = 1f;
            if (0f <= time && (time <= 1f || key.isLeaf))
            {
                r = key.shape.Evaluate(time);
                r = 1 - (1-r) * key.weight;
            }
            result = Vector3.Lerp(result, v, r);
            return (result, time, distance);
        }

    }
}
