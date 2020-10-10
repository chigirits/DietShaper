﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chigiri.DietShaper
{

	[DisallowMultipleComponent]
	[HelpURL("https://github.com/chigirits/DietShaper")]
    public class DietShaper : MonoBehaviour
    {

        public Animator avatarRoot;
        public SkinnedMeshRenderer targetRenderer;
        public Mesh sourceMesh;
        public bool alwaysShowGizmo;

        public ShapeKey[] shapeKeys = new ShapeKey[] {

            new ShapeKey {
                name = "Diet.Hips",
                radius = 0.15f,
                gizmoColor = Color.blue,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0f, 0.35f, SignRange.both, HumanBodyBones.Hips, HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg),
                    new BodyLine(0f, 0.35f, SignRange.both, HumanBodyBones.Hips, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg)
                }
            },

            new ShapeKey {
                name = "Diet.Spine",
                radius = 0.14f,
                gizmoColor = Color.cyan,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0f, 0f, SignRange.both, HumanBodyBones.Hips, HumanBodyBones.Spine, HumanBodyBones.Chest)
                }
            },

            new ShapeKey {
                name = "Diet.Chest",
                radius = 0.13f,
                gizmoColor = Color.green,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0.35f, 0f, SignRange.both, HumanBodyBones.Spine, HumanBodyBones.Chest, HumanBodyBones.Neck)
                }
            },

            new ShapeKey {
                name = "Diet.Shoulders",
                radius = 0.08f,
                addNormal = 0.05f,
                gizmoColor = Color.yellow,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0.35f, 0.35f, SignRange.both, HumanBodyBones.Neck, HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm),
                    new BodyLine(0.35f, 0.35f, SignRange.both, HumanBodyBones.Neck, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm)
                }
            },

            new ShapeKey {
                name = "Diet.UpperArms",
                radius = 0.07f,
                gizmoColor = Color.Lerp(Color.red, Color.yellow, 0.5f),
                bodyLines = new List<BodyLine>{
                    new BodyLine(0f, 0f, SignRange.both, HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm),
                    new BodyLine(0f, 0f, SignRange.both, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm)
                }
            },

            new ShapeKey {
                name = "Diet.Elbows",
                radius = 0.06f,
                gizmoColor = Color.red,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0.35f, 0.35f, SignRange.both, HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand),
                    new BodyLine(0.35f, 0.35f, SignRange.both, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand)
                }
            },

            new ShapeKey {
                name = "Diet.LowerArms",
                radius = 0.05f,
                gizmoColor = Color.magenta,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0f, 0.15f, SignRange.both, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand),
                    new BodyLine(0f, 0.15f, SignRange.both, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand)
                }
            },

            new ShapeKey {
                name = "Diet.Wrists",
                radius = 0.06f,
                gizmoColor = Color.blue,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0.35f, 0.35f, SignRange.both, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand, HumanBodyBones.LeftMiddleDistal),
                    new BodyLine(0.35f, 0.35f, SignRange.both, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand, HumanBodyBones.RightMiddleDistal)
                }
            },

            new ShapeKey {
                name = "Diet.Hands",
                radius = 0.07f,
                isLeaf = true,
                gizmoColor = Color.cyan,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0f, 0f, SignRange.both, HumanBodyBones.LeftHand, HumanBodyBones.LeftMiddleDistal),
                    new BodyLine(0f, 0f, SignRange.both, HumanBodyBones.RightHand, HumanBodyBones.RightMiddleDistal)
                },
                shape = new AnimationCurve(
                    new Keyframe(0f, 1f),
                    new Keyframe(0.2f, 0f),
                    new Keyframe(1f, 0f)
                )
            },

            new ShapeKey {
                name = "Diet.UpperLegs",
                radius = 0.1f,
                gizmoColor = Color.magenta,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0f, 0f, SignRange.negative, HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg),
                    new BodyLine(0f, 0f, SignRange.positive, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg)
                }
            },

            new ShapeKey {
                name = "Diet.Knees",
                radius = 0.09f,
                gizmoColor = Color.red,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0.35f, 0.35f, SignRange.negative, HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot),
                    new BodyLine(0.35f, 0.35f, SignRange.positive, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot)
                }
            },

            new ShapeKey {
                name = "Diet.LowerLegs",
                radius = 0.08f,
                gizmoColor = Color.Lerp(Color.red, Color.yellow, 0.5f),
                bodyLines = new List<BodyLine>{
                    new BodyLine(0f, 0f, SignRange.negative, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot),
                    new BodyLine(0f, 0f, SignRange.positive, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot)
                }
            },

            new ShapeKey {
                name = "Diet.Ankles",
                radius = 0.07f,
                gizmoColor = Color.yellow,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0.35f, 0.15f, SignRange.negative, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot, HumanBodyBones.LeftToes),
                    new BodyLine(0.35f, 0.15f, SignRange.positive, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot, HumanBodyBones.RightToes)
                }
            },

            new ShapeKey {
                name = "Diet.Feet",
                radius = 0.09f,
                isLeaf = true,
                gizmoColor = Color.green,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0.35f, 0f, SignRange.negative, HumanBodyBones.LeftFoot, HumanBodyBones.LeftToes),
                    new BodyLine(0.35f, 0f, SignRange.positive, HumanBodyBones.RightFoot, HumanBodyBones.RightToes)
                },
                shape = new AnimationCurve(
                    new Keyframe(0f, 1f),
                    new Keyframe(0.2f, 0f),
                    new Keyframe(1f, 0f)
                )
            }

        };

#if UNITY_EDITOR

        void OnDrawGizmosSelected()
        {
            if (alwaysShowGizmo) return;
            if (avatarRoot == null) return;
            foreach (var key in shapeKeys) key.DrawGizmos(avatarRoot);
        }

        void OnDrawGizmos()
        {
            if (!alwaysShowGizmo) return;
            if (avatarRoot == null) return;
            foreach (var key in shapeKeys) key.DrawGizmos(avatarRoot);
        }

#endif // UNITY_EDITOR

    }

}
