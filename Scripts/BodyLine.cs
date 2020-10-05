using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace Chigiri.DietShaper
{

    [System.Serializable]
    public class BodyLine
    {

        public List<HumanBodyBones> bones;
        public bool isLeaf;
        public float startMargin;
        public float endMargin;

        public int _index;
        public bool _isOpen;

        public BodyLine(float startMargin, float endMargin, params HumanBodyBones[] bones)
        {
            this.startMargin = startMargin;
            this.endMargin = endMargin;
            this.bones = new List<HumanBodyBones>(bones);
            var last = this.bones.Count - 1;
            if (this.bones[last] == HumanBodyBones.LastBone)
            {
                this.bones.RemoveAt(last);
                isLeaf = true;
            }
        }

#if UNITY_EDITOR

        // Reference: https://forum.unity.com/threads/drawing-capsule-gizmo.354634/#post-4100557
        public static void DrawWireTube(Vector3 center, Quaternion rotation, float radius, float height)
        {
            if (height == 0f) return;
            Matrix4x4 angleMatrix = Matrix4x4.TRS(center, rotation, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                var n = 8;
                for (var i=0; i<n; i++)
                {
                    var a = Mathf.PI * 2f * i / n;
                    var x = Mathf.Cos(a) * radius;
                    var y = Mathf.Sin(a) * radius;
                    Handles.DrawLine(new Vector3(x, y, height/2f), new Vector3(x, y, -height/2f));
                }
            }
        }

        public static void DrawWireDisc(Vector3 center, Quaternion rotation, float radius)
        {
            Matrix4x4 angleMatrix = Matrix4x4.TRS(center, rotation, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                Handles.DrawWireDisc(Vector3.zero, Vector3.forward, radius);
            }
        }

        public static void DrawWireHalfDisc(Vector3 center, Quaternion rotation, float sign, float radius)
        {
            Matrix4x4 angleMatrix = Matrix4x4.TRS(center, rotation, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                Handles.DrawWireArc(Vector3.zero, Vector3.forward, Vector3.up, sign * 180f, radius);
            }
        }

        public static void DrawComb(Vector3 center, Quaternion rotation, float angle, float radius)
        {
            Matrix4x4 angleMatrix = Matrix4x4.TRS(center, rotation, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                Handles.DrawWireDisc(Vector3.zero, Vector3.left, radius);
                Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, angle/2f, radius);
                Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, -angle/2f, radius);

                var a = angle/2f * Mathf.PI / 180f;
                var c = Mathf.Cos(a) * radius;
                var s = Mathf.Sin(a) * radius;
                Handles.DrawWireArc(Vector3.zero, new Vector3(c, 0f, s), Vector3.up, 180f, radius);
                Handles.DrawWireArc(Vector3.zero, new Vector3(-c, 0f, s), Vector3.up, -180f, radius);
            }
        }

        public void DrawGizmos(Animator avatarRoot, float radius, Color color)
        {
            Handles.color = color;
            Gizmos.color = color;
            var rm = bones.Count - 1;
            for (var i = 1; i < bones.Count; i++)
            {
                var bone0 = avatarRoot.GetBoneTransform(bones[i-1]);
                var bone1 = avatarRoot.GetBoneTransform(bones[i]);
                var b0 = bone0.position;
                var b1 = bone1.position;
                var begin = i == 1 ? Vector3.Lerp(b0, b1, startMargin*rm) : b0;
                var end = i == bones.Count-1 ? Vector3.Lerp(b1, b0, endMargin*rm) : b1;
                var center = (begin + end) * 0.5f;
                var beginToEnd = end - begin;
                var v01 = beginToEnd.normalized;
                var rot01 = Quaternion.LookRotation(beginToEnd, bone0.up);
                var length = beginToEnd.magnitude;

                // 円筒の側面
                DrawWireTube(center, rot01, radius, length);

                // 円筒の中線
                if (isLeaf)
                    Gizmos.DrawRay(begin, beginToEnd * 1e4f);
                else
                    Gizmos.DrawLine(begin, end);

                if (i == 1) DrawWireDisc(begin, rot01, radius); // 全体の開始点の円
                if (i == bones.Count - 1) DrawWireDisc(end, rot01, radius); // 全体の終了点の円

                // 関節
                if (i < bones.Count - 1)
                {
                    var bone2 = avatarRoot.GetBoneTransform(bones[i+1]);
                    var b2 = bone2.position;
                    var v12 = (b2 - b1).normalized;
                    var rotM = Quaternion.LookRotation(v01-v12, Vector3.Cross(v01, -v12));
                    var rot12 = Quaternion.LookRotation(v12, bone1.up);
                    var angle = 180f - Mathf.Acos(Vector3.Dot(v01, -v12)) * 180f / Mathf.PI;
                    var sign = Mathf.Sign(Vector3.Dot(bone1.up, Vector3.Cross(v01, v12)));
                    if (angle < 5f)
                    {
                        DrawWireDisc(b1, rot01, radius);
                        DrawWireDisc(b1, rot12, radius);
                    }
                    else
                    {
                        // 関節
                        DrawComb(b1, rotM, angle, radius);
                        // // 直前の円筒終了点の半円
                        // DrawWireHalfDisc(b1, rot01, sign, radius);
                        // // 次の円筒開始点の半円
                        // DrawWireHalfDisc(b1, rot12, sign, radius);
                    }
                }
            }
        }

#endif // UNITY_EDITOR

    }

}
