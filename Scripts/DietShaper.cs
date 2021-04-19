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
        public bool isGenericMode;
        public bool adjustScale;
        public bool alwaysShowGizmo;
        public List<ShapeKey> shapeKeys = new List<ShapeKey> {};

#if UNITY_EDITOR

        public string Validate(List<string> warnings)
        {
            if (avatarRoot == null)
            {
                return "Avatar Root を指定してください";
            }
            if (targetRenderer == null)
            {
                return "Target を指定してください";
            }
            if (!targetRenderer.transform.IsChildOf(avatarRoot.transform))
            {
                return "Target は Avatar Root の階層下にある必要があります";
            }
            if (sourceMesh == null)
            {
                return "Source Mesh を指定してください";
            }
            if (!avatarRoot.isHuman && !isGenericMode)
            {
                return "Avatar Root が Humanoid でないときは Generic Mode を有効にする必要があります";
            }
            if (shapeKeys.Count == 0)
            {
                return "Add Shape Key From Preset から1つ以上のプリセットを追加してください";
            }

            var ascale = avatarRoot.transform.lossyScale;
            var rscale = targetRenderer.localToWorldMatrix.lossyScale;
            if (0.001f < Mathf.Abs(rscale.magnitude / ascale.magnitude - 1f))
            {
                warnings.Add("Target のスケールが標準的ではありません。" +
                    "シェイプキーの変化量が異常になる場合は Adjust Scale をチェックしてください。" +
                    "それでもうまく行かない場合は、マニュアルの注意事項をお読みください。");
            }

            var names = new Dictionary<string, int>();
            foreach (var shapeKey in shapeKeys)
            {
                if (!shapeKey.enable) continue;
                var name = shapeKey.name;
                if (!names.ContainsKey(name)) names[name] = 0;
                if (names[name] == 1)
                {
                    warnings.Add($"シェイプキー名 {name} が重複しています。" +
                        "これらの設定は合成され、単一のシェイプキーとして登録されます。");
                }
                names[name]++;
                for (var j = 0; j < sourceMesh.blendShapeCount; j++)
                {
                    if (name == sourceMesh.GetBlendShapeName(j))
                    {
                        return $"Source Mesh には {name} というシェイプキーが既に定義されています。 " +
                            "処理済みのメッシュが指定されていないかを確認してください。\n" +
                            "・処理をやり直す場合は、オリジナルのメッシュを指定しなおしてください。\n" +
                            "・処理済みのメッシュにさらに新しいキーを追加する場合は、名前を変更してください。";
                    }
                }
            }

            var i = 0;
            foreach (var shapeKey in shapeKeys)
            {
                var err = shapeKey.Validate(avatarRoot, isGenericMode, i);
                if (err != "") return err;
                i++;
            }

            return "";
        }

        void OnDrawGizmosSelected()
        {
            if (alwaysShowGizmo) return;
            if (avatarRoot == null) return;
            foreach (var key in shapeKeys) key.DrawGizmos(avatarRoot, isGenericMode);
        }

        void OnDrawGizmos()
        {
            if (!alwaysShowGizmo) return;
            if (avatarRoot == null) return;
            foreach (var key in shapeKeys) key.DrawGizmos(avatarRoot, isGenericMode);
        }

#endif // UNITY_EDITOR

    }

}
