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
        public float radius = 0.1f;
        public float startMargin;
        public float endMargin;
        public bool isLeaf;
        public AnimationCurve shape = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(0.2f, 0f),
            new Keyframe(0.8f, 0f),
            new Keyframe(1f, 1f)
        );
        public float addNormal;
        public float removeThreshold = 0.0f;
        public Color gizmoColor = Color.green;

        public static string[] presetKeys = new string[] {
            "All",
            "---------",
            "Hips",
            "Spine",
            "Chest",
            "Shoulders",
            "UpperArms",
            "Elbows",
            "LowerArms",
            "Wrists",
            "Hands",
            "UpperLegs",
            "Knees",
            "LowerLegs",
            "Ankles",
            "Feet"
        };

        public static Dictionary<string, ShapeKey> presets = new Dictionary<string, ShapeKey> {

            ["Hips"] = new ShapeKey {
                name = "Diet.Hips",
                radius = 0.15f,
                endMargin = 0.35f,
                gizmoColor = Color.blue,
                bodyLines = new List<BodyLine>{
                    new BodyLine(SignRange.both, HumanBodyBones.Hips, HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg),
                    new BodyLine(SignRange.both, HumanBodyBones.Hips, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg)
                }
            },

            ["Spine"] = new ShapeKey {
                name = "Diet.Spine",
                radius = 0.14f,
                gizmoColor = Color.cyan,
                bodyLines = new List<BodyLine>{
                    new BodyLine(SignRange.both, HumanBodyBones.Hips, HumanBodyBones.Spine, HumanBodyBones.Chest)
                }
            },

            ["Chest"] = new ShapeKey {
                name = "Diet.Chest",
                radius = 0.13f,
                startMargin = 0.35f,
                endMargin = 0.2f,
                gizmoColor = Color.green,
                bodyLines = new List<BodyLine>{
                    new BodyLine(SignRange.both, HumanBodyBones.Spine, HumanBodyBones.Chest, HumanBodyBones.Neck)
                }
            },

            ["Shoulders"] = new ShapeKey {
                name = "Diet.Shoulders",
                radius = 0.08f,
                startMargin = 0.35f,
                endMargin = 0.35f,
                addNormal = 0.05f,
                gizmoColor = Color.yellow,
                bodyLines = new List<BodyLine>{
                    new BodyLine(SignRange.both, HumanBodyBones.Neck, HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm),
                    new BodyLine(SignRange.both, HumanBodyBones.Neck, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm)
                }
            },

            ["UpperArms"] = new ShapeKey {
                name = "Diet.UpperArms",
                radius = 0.07f,
                gizmoColor = Color.Lerp(Color.red, Color.yellow, 0.5f),
                bodyLines = new List<BodyLine>{
                    new BodyLine(SignRange.both, HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm),
                    new BodyLine(SignRange.both, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm)
                }
            },

            ["Elbows"] = new ShapeKey {
                name = "Diet.Elbows",
                radius = 0.06f,
                startMargin = 0.35f,
                endMargin = 0.35f,
                gizmoColor = Color.red,
                bodyLines = new List<BodyLine>{
                    new BodyLine(SignRange.both, HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand),
                    new BodyLine(SignRange.both, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand)
                }
            },

            ["LowerArms"] = new ShapeKey {
                name = "Diet.LowerArms",
                radius = 0.05f,
                endMargin = 0.15f,
                gizmoColor = Color.magenta,
                bodyLines = new List<BodyLine>{
                    new BodyLine(SignRange.both, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand),
                    new BodyLine(SignRange.both, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand)
                }
            },

            ["Wrists"] = new ShapeKey {
                name = "Diet.Wrists",
                radius = 0.06f,
                startMargin = 0.35f,
                endMargin = 0.35f,
                gizmoColor = Color.blue,
                bodyLines = new List<BodyLine>{
                    new BodyLine(SignRange.both, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand, HumanBodyBones.LeftMiddleDistal),
                    new BodyLine(SignRange.both, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand, HumanBodyBones.RightMiddleDistal)
                }
            },

            ["Hands"] = new ShapeKey {
                name = "Diet.Hands",
                radius = 0.07f,
                isLeaf = true,
                gizmoColor = Color.cyan,
                bodyLines = new List<BodyLine>{
                    new BodyLine(SignRange.both, HumanBodyBones.LeftHand, HumanBodyBones.LeftMiddleDistal),
                    new BodyLine(SignRange.both, HumanBodyBones.RightHand, HumanBodyBones.RightMiddleDistal)
                },
                shape = new AnimationCurve(
                    new Keyframe(0f, 1f),
                    new Keyframe(0.2f, 0f),
                    new Keyframe(1f, 0f)
                )
            },

            ["UpperLegs"] = new ShapeKey {
                name = "Diet.UpperLegs",
                radius = 0.1f,
                gizmoColor = Color.magenta,
                bodyLines = new List<BodyLine>{
                    new BodyLine(SignRange.negative, HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg),
                    new BodyLine(SignRange.positive, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg)
                }
            },

            ["Knees"] = new ShapeKey {
                name = "Diet.Knees",
                radius = 0.09f,
                startMargin = 0.35f,
                endMargin = 0.35f,
                gizmoColor = Color.red,
                bodyLines = new List<BodyLine>{
                    new BodyLine(SignRange.negative, HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot),
                    new BodyLine(SignRange.positive, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot)
                }
            },

            ["LowerLegs"] = new ShapeKey {
                name = "Diet.LowerLegs",
                radius = 0.08f,
                gizmoColor = Color.Lerp(Color.red, Color.yellow, 0.5f),
                bodyLines = new List<BodyLine>{
                    new BodyLine(SignRange.negative, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot),
                    new BodyLine(SignRange.positive, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot)
                }
            },

            ["Ankles"] = new ShapeKey {
                name = "Diet.Ankles",
                radius = 0.07f,
                startMargin = 0.35f,
                endMargin = 0.15f,
                gizmoColor = Color.yellow,
                bodyLines = new List<BodyLine>{
                    new BodyLine(SignRange.negative, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot, HumanBodyBones.LeftToes),
                    new BodyLine(SignRange.positive, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot, HumanBodyBones.RightToes)
                }
            },

            ["Feet"] = new ShapeKey {
                name = "Diet.Feet",
                radius = 0.09f,
                startMargin = 0.35f,
                isLeaf = true,
                gizmoColor = Color.green,
                bodyLines = new List<BodyLine>{
                    new BodyLine(SignRange.negative, HumanBodyBones.LeftFoot, HumanBodyBones.LeftToes),
                    new BodyLine(SignRange.positive, HumanBodyBones.RightFoot, HumanBodyBones.RightToes)
                },
                shape = new AnimationCurve(
                    new Keyframe(0f, 1f),
                    new Keyframe(0.2f, 0f),
                    new Keyframe(1f, 0f)
                )
            }

        };

#if UNITY_EDITOR

        public void DrawGizmos(Animator avatarRoot)
        {
            if (!enable) return;
            foreach (var bodyLine in bodyLines)
            {
                bodyLine.DrawGizmos(avatarRoot, startMargin, endMargin, radius, radius, isLeaf, gizmoColor);
            }
        }

#endif // UNITY_EDITOR

    }
}
