﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace Chigiri.DietShaper
{

    public enum SignRange
    {
        both = 0,
        positive = 1,
        negative = 2
    }

    [System.Serializable]
    public class BodyLine
    {

        public List<HumanBodyBones> bones;
        public List<Transform> genericBones;
        public SignRange xSignRange;

        public int _index;
        public bool _isGenericMode = false;

        public BodyLine(SignRange xSignRange, params HumanBodyBones[] bones)
        {
            this.xSignRange = xSignRange;
            this.bones = new List<HumanBodyBones>(bones);
            genericBones = new List<Transform>();
            for (var i=0; i<bones.Length; i++) genericBones.Add(null);
        }

        public Transform GetBoneTransform(int index, Animator avatarRoot, bool isGenericMode)
        {
            if (isGenericMode)
            {
                return index < genericBones.Count ? genericBones[index] : null;
            }
            return index < bones.Count ? avatarRoot.GetBoneTransform(bones[index]) : null;
        }

#if UNITY_EDITOR

        public string Validate(Animator avatarRoot, bool isGenericMode, string shapeKeyName, int bodyLineIndex)
        {
            if (isGenericMode)
            {
                var i = 0;
                foreach (var genericBone in genericBones)
                {
                    if (genericBone == null) return $"{shapeKeyName} > Body Lines[{bodyLineIndex}] > Bones[{i}] を指定してください";
                    i++;
                }
            }
            else
            {
                var i = 0;
                foreach (var bone in bones)
                {
                    var t = avatarRoot.GetBoneTransform(bone);
                    if (t == null) return $"{shapeKeyName} > Body Lines[{bodyLineIndex}] > Bones[{i}] に対応するボーンが Avatar Root 内に見つかりません。代替となるボーンを選択してください";
                    i++;
                }
            }
            return "";
        }

        // Reference: https://forum.unity.com/threads/drawing-capsule-gizmo.354634/#post-4100557
        public static void DrawWireTube(Vector3 position, Quaternion rotation, float radius0, float radius1, float height)
        {
            if (height == 0f) return;
            Matrix4x4 angleMatrix = Matrix4x4.TRS(position, rotation, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                var n = 8;
                for (var i=0; i<n; i++)
                {
                    var a = Mathf.PI * 2f * i / n;
                    Handles.DrawLine(
                        new Vector3(
                            Mathf.Cos(a) * radius0,
                            Mathf.Sin(a) * radius0,
                            -height/2f
                        ),
                        new Vector3(
                            Mathf.Cos(a) * radius1,
                            Mathf.Sin(a) * radius1,
                            height/2f
                        )
                    );
                }
            }
        }

        public static void DrawWireDisc(Vector3 position, Quaternion rotation, float radius)
        {
            Matrix4x4 angleMatrix = Matrix4x4.TRS(position, rotation, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                Handles.DrawWireDisc(Vector3.zero, Vector3.forward, radius);
            }
        }

        public static void DrawWireHalfDisc(Vector3 position, Quaternion rotation, float sign, float radius)
        {
            Matrix4x4 angleMatrix = Matrix4x4.TRS(position, rotation, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                Handles.DrawWireArc(Vector3.zero, Vector3.forward, Vector3.up, sign * 180f, radius);
            }
        }

        public static void DrawComb(Vector3 position, Quaternion rotation, float angle, float radius)
        {
            Matrix4x4 angleMatrix = Matrix4x4.TRS(position, rotation, Handles.matrix.lossyScale);
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

        public void DrawGizmos(Animator avatarRoot, float startMargin, float endMargin, float startRadius, float endRadius, bool isLeaf, bool isGenericMode, Color color)
        {
            Handles.color = color;
            Gizmos.color = color;
            var rm = bones.Count - 1;
            for (var i = 1; i < bones.Count; i++)
            {
                var radius0 = Mathf.Lerp(startRadius, endRadius, (float)(i-1)/rm);
                var radius1 = Mathf.Lerp(startRadius, endRadius, (float)i/rm);
                var bone0 = GetBoneTransform(i-1, avatarRoot, isGenericMode);
                var bone1 = GetBoneTransform(i, avatarRoot, isGenericMode);
                if (bone0 == null || bone1 == null) continue;
                var b0 = bone0.position;
                var b1 = bone1.position;
                var begin = i == 1 ? Vector3.Lerp(b0, b1, startMargin*rm) : b0;
                var end = i == bones.Count-1 ? Vector3.Lerp(b1, b0, endMargin*rm) : b1;
                var position = (begin + end) * 0.5f;
                var beginToEnd = end - begin;
                if (beginToEnd.sqrMagnitude == 0f) continue; // TODO: 隣接ボーンの座標が完全に一致するときの代替処理
                var v01 = beginToEnd.normalized;
                var rot01 = Quaternion.LookRotation(beginToEnd, bone0.up);
                var length = beginToEnd.magnitude;

                // 円筒の側面
                DrawWireTube(position, rot01, radius0, radius1, length);

                // 円筒の中線
                if (isLeaf)
                    Gizmos.DrawRay(begin, beginToEnd * 1e4f);
                else
                    Gizmos.DrawLine(begin, end);

                if (i == 1) DrawWireDisc(begin, rot01, radius0); // 全体の開始点の円
                if (i == bones.Count - 1) DrawWireDisc(end, rot01, radius1); // 全体の終了点の円

                // 関節
                if (i < bones.Count - 1)
                {
                    var bone2 = GetBoneTransform(i+1, avatarRoot, isGenericMode);
                    if (bone2 == null) continue;
                    var b2 = bone2.position;
                    var v12 = (b2 - b1).normalized;
                    if (v01 == v12) continue;
                    var rotM = Quaternion.LookRotation(v01-v12, Vector3.Cross(v01, -v12));
                    var rot12 = Quaternion.LookRotation(v12, bone1.up);
                    var angle = 180f - Mathf.Acos(Vector3.Dot(v01, -v12)) * 180f / Mathf.PI;
                    var sign = Mathf.Sign(Vector3.Dot(bone1.up, Vector3.Cross(v01, v12)));
                    if (angle < 5f)
                    {
                        DrawWireDisc(b1, rot01, radius1);
                        DrawWireDisc(b1, rot12, radius1);
                    }
                    else
                    {
                        // 関節
                        DrawComb(b1, rotM, angle, radius1);
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
