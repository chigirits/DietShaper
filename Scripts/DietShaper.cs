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
                    new BodyLine(0f, 0.35f, HumanBodyBones.Hips, HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg),
                    new BodyLine(0f, 0.35f, HumanBodyBones.Hips, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg)
                }
            },

            new ShapeKey {
                name = "Diet.Spine",
                radius = 0.15f,
                gizmoColor = Color.cyan,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0f, 0f, HumanBodyBones.Spine, HumanBodyBones.Chest)
                }
            },

            new ShapeKey {
                name = "Diet.Chest",
                radius = 0.15f,
                gizmoColor = Color.green,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0f, 0f, HumanBodyBones.Chest, HumanBodyBones.Neck)
                }
            },

            new ShapeKey {
                name = "Diet.Shoulders",
                // enable = false,
                radius = 0.08f,
                gizmoColor = Color.yellow,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0.35f, 0.35f, HumanBodyBones.Neck, HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm),
                    new BodyLine(0.35f, 0.35f, HumanBodyBones.Neck, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm)
                }
            },

            new ShapeKey {
                name = "Diet.UpperArms",
                radius = 0.05f,
                gizmoColor = Color.Lerp(Color.red, Color.yellow, 0.5f),
                bodyLines = new List<BodyLine>{
                    new BodyLine(0f, 0f, HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm),
                    new BodyLine(0f, 0f, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm)
                }
            },

            new ShapeKey {
                name = "Diet.Elbows",
                radius = 0.05f,
                gizmoColor = Color.red,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0.35f, 0.35f, HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand),
                    new BodyLine(0.35f, 0.35f, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand)
                }
            },

            new ShapeKey {
                name = "Diet.LowerArms",
                radius = 0.05f,
                gizmoColor = Color.magenta,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0f, 0f, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand),
                    new BodyLine(0f, 0f, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand)
                }
            },

            new ShapeKey {
                name = "Diet.Hands",
                radius = 0.05f,
                gizmoColor = Color.blue,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0f, 0f, HumanBodyBones.LeftHand, HumanBodyBones.LeftMiddleDistal, HumanBodyBones.LastBone),
                    new BodyLine(0f, 0f, HumanBodyBones.RightHand, HumanBodyBones.RightMiddleDistal, HumanBodyBones.LastBone)
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
                    new BodyLine(0f, 0f, HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg),
                    new BodyLine(0f, 0f, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg)
                }
            },

            new ShapeKey {
                name = "Diet.Knees",
                radius = 0.1f,
                gizmoColor = Color.red,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0.35f, 0.35f, HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot),
                    new BodyLine(0.35f, 0.35f, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot)
                }
            },

            new ShapeKey {
                name = "Diet.LowerLegs",
                radius = 0.1f,
                gizmoColor = Color.Lerp(Color.red, Color.yellow, 0.5f),
                bodyLines = new List<BodyLine>{
                    new BodyLine(0f, 0f, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot),
                    new BodyLine(0f, 0f, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot)
                }
            },

            new ShapeKey {
                name = "Diet.Feet",
                radius = 0.1f,
                gizmoColor = Color.yellow,
                bodyLines = new List<BodyLine>{
                    new BodyLine(0f, 0f, HumanBodyBones.LeftFoot, HumanBodyBones.LeftToes, HumanBodyBones.LastBone),
                    new BodyLine(0f, 0f, HumanBodyBones.RightFoot, HumanBodyBones.RightToes, HumanBodyBones.LastBone)
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
