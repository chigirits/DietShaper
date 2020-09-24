using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chigiri.DietShaper.Editor
{
    public class NearestPointResolver
    {

        public class BoneGroup
        {

            Vector3 begin;
            Vector3 v;
            Vector3 vIdent;
            float vLength;
            bool limitEnd;

            public BoneGroup(Vector3 begin, Vector3 end, bool limitEnd)
            {
                this.begin = begin;
                v = end - begin;
                vIdent = v.normalized;
                vLength = v.magnitude;
                this.limitEnd = limitEnd;
            }

            // 頂点からボーン線分に下ろした垂線の足を求める
            public (Vector3, float, float) NearestPoint(Vector3 vertex)
            {
                var t = Vector3.Dot(vIdent, vertex - begin) / vLength;
                if (t < 0f || 1f < t && limitEnd) return (vertex, 0f, Mathf.Infinity); // 垂線の足がボーン線分上になければ変形しない
                var nearest = begin + v * t;
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
                var begin = avatarRoot.GetBoneTransform(b.bones[0]).position;
                var end = avatarRoot.GetBoneTransform(b.bones[1]).position;
                groups[i] = new BoneGroup(begin, end, !b.isLeaf);
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
                var (n, t, d) = groups[i].NearestPoint(v);
                if (distance <= d) continue;
                distance = d;
                time = t;
                result = n;
            }
            result = Vector3.Lerp(result, v, key.shape.Evaluate(time));
            return (result, distance);
        }

    }
}
