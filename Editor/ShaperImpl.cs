using UnityEngine;
using UnityEditor;
using System.IO;

namespace Chigiri.DietShaper.Editor
{
    public class ShaperImpl
    {

        public static void Process(DietShaper p)
        {
            var resultMesh = DoProcess(p);
            if (resultMesh == null) return;
            resultMesh.name = p.sourceMesh.name + ".DietShaped";

            // 保存ダイアログを表示
            string dir = AssetDatabase.GetAssetPath(p.sourceMesh);
            dir = dir == "" ? "Assets" : Path.GetDirectoryName(dir);
            string path = EditorUtility.SaveFilePanel("Save the new mesh as", dir, Helper.SanitizeFileName(resultMesh.name), "asset");
            if (path.Length == 0) return;

            // 保存
            if (!path.StartsWith(Application.dataPath))
            {
                Debug.LogError("Invalid path: Path must be under " + Application.dataPath);
                return;
            }
            path = path.Replace(Application.dataPath, "Assets");
            AssetDatabase.CreateAsset(resultMesh, path);
            Debug.Log("Asset exported: " + path);

            // Targetのメッシュを差し替えてシェイプキーのウェイトを設定
            Undo.RecordObject(p.targetRenderer, "Process (DietShaper)");
            p.targetRenderer.sharedMesh = resultMesh;
            Selection.activeGameObject = p.targetRenderer.gameObject;
        }

        static void AddBoneKey(DietShaper p, ShapeKey key, ShapeKey exceptKey, Vector3[] posed, Vector3[] normals, Mesh result)
        {
            if (!key.enable) return;
            var vertices = new Vector3[p.sourceMesh.vertexCount];
            var tr = p.targetRenderer.transform;
            var resolver = new NearestPointResolver(p.avatarRoot, key);
            for (var j = 0; j < p.sourceMesh.vertexCount; j++)
            {
                var v = tr.TransformPoint(posed[j]);
                var (nearest, time, distance) = resolver.Resolve(v);
                var radius = key.radius; // Mathf.Lerp(key.startRadius, key.endRadius, time);
                if (radius < distance) continue;
                var w = tr.InverseTransformVector(nearest - v); // 結果の座標差分

                // 法線をミックス
                if (0f < key.addNormal && 0f <= time && time <= 1f)
                {
                    var n = -normals[j].normalized; // 法線の逆方向
                    var nt = n - Vector3.Project(n, w.normalized); // nのwに対して垂直な成分
                    w += nt * key.addNormal * (1f - key.shape.Evaluate(time));
                }

                vertices[j] = w;
            }
            result.AddBlendShapeFrame(key.name, 100f, vertices, null, null);
            //var index = result.GetBlendShapeIndex(key.name);
            //p.targetRenderer.SetBlendShapeWeight(index, 100f);
        }

        static Mesh DoProcess(DietShaper p)
        {
            var posed = Helper.GetPosedVertices(p.targetRenderer, p.sourceMesh);
            var result = Object.Instantiate(p.sourceMesh);
            result.name = p.sourceMesh.name;

            foreach (var key in p.shapeKeys)
            {
                AddBoneKey(p, key, null, posed, p.sourceMesh.normals, result);
            }

            return result;
        }

    }
}
