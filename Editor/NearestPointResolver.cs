using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chigiri.DietShaper.Editor
{
    public class NearestPointResolver
    {

        interface BoneGroup
        {
            (Vector3, float, float) NearestPoint(Vector3 vertex);
        }

        public class BoneGroup2 : BoneGroup
        {

            Vector3 start;
            Vector3 v;
            Vector3 vIdent;
            float vLength;
            bool limitEnd;

            public BoneGroup2(Vector3 start, Vector3 end, bool limitEnd)
            {
                this.start = start;
                v = end - start;
                vIdent = v.normalized;
                vLength = v.magnitude;
                this.limitEnd = limitEnd;
            }

            public (Vector3, float, float) NearestPoint(Vector3 vertex)
            {
                // 頂点からボーン線分に下ろした垂線の足を求める
                var t = Vector3.Dot(vIdent, vertex - start) / vLength;
                if (t < 0f || 1f < t && limitEnd) return (vertex, 0f, Mathf.Infinity); // 垂線の足がボーン線分上になければ変形しない
                var nearest = start + v * t;
                var distance = (vertex - nearest).magnitude;
                return (nearest, t, distance);
            }

        }

        Animator avatarRoot;
        ShapeKey key;
        BoneGroup[] groups;

        public NearestPointResolver(Animator avatarRoot, ShapeKey key)
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
                        var start = avatarRoot.GetBoneTransform(b.bones[0]).position;
                        var end = avatarRoot.GetBoneTransform(b.bones[1]).position;
                        groups[i] = new BoneGroup2(start, end, !b.isLeaf);
                        break;
                    default:
                        Debug.Log("Body line must have up to 3 bones");
                        break;
                }
            }
        }

        // 最も近いボーン線分への垂線の足を求める
        public (Vector3, float) Resolve(Vector3 v)
        {
            var distance = Mathf.Infinity;
            var result = v;
            var time = 0f;
            for (var i = 0; i < key.bodyLines.Count; i++)
            {
                var bodyLine = key.bodyLines[i];
                var (p, t, d) = groups[i].NearestPoint(v);
                if (distance <= d) continue; // 最も近いbodyLineだけを採用
                result = p;
                distance = d;
                // startMargin, endMargin を考慮してエンベロープ内の位置(time)を調整
                var m0 = bodyLine.startMargin;
                var m1 = bodyLine.endMargin;
                var mr = 1f - m0 - m1;
                if (mr < 1e-5)
                    time = 0.5f;
                else
                    time = (t - m0) / mr;
            }
            time = Mathf.Clamp(time, 0f, 1f);
            result = Vector3.Lerp(result, v, key.shape.Evaluate(time));
            return (result, distance);
        }

    }
}
