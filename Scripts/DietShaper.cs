using System.Collections;
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
        public List<ShapeKey> shapeKeys = new List<ShapeKey> {};

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
