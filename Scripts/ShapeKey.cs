using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chigiri.DietShaper
{

    [System.Serializable]
    public class ShapeKey
    {

        public bool enable = true;
        public string name = "";
        public List<BodyLine> bodyLines = new List<BodyLine>();
        public float startRadius = 0.1f;
        public float endRadius = 0.1f;
        public float addNormal;
        public bool isLeaf;
        public AnimationCurve shape = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(0.2f, 0f),
            new Keyframe(0.8f, 0f),
            new Keyframe(1f, 1f)
        );
        public Color gizmoColor = Color.green;

        public bool _isOpen;

#if UNITY_EDITOR

        public void DrawGizmos(Animator avatarRoot)
        {
            if (!enable) return;
            foreach (var bodyLine in bodyLines)
            {
                bodyLine.DrawGizmos(avatarRoot, startRadius, endRadius, isLeaf, gizmoColor);
            }
        }

#endif // UNITY_EDITOR

    }
}
