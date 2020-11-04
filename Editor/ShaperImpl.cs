using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

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
            key.shape.preWrapMode = WrapMode.ClampForever;
            key.shape.postWrapMode = WrapMode.ClampForever;
            var vertices = new Vector3[p.sourceMesh.vertexCount];
            var tr = p.targetRenderer.transform;
            var resolver = new NearestPointResolver(p.avatarRoot, key, p.isGenericMode);
            var toBeRemoved = new bool[p.sourceMesh.vertexCount];
            for (var j = 0; j < p.sourceMesh.vertexCount; j++)
            {
                var v = posed[j];
                var (nearest, time, distance) = resolver.Resolve(v);
                var radius = key.radius; // Mathf.Lerp(key.startRadius, key.endRadius, time);
                if (radius < distance) continue;
                var w = tr.InverseTransformVector(nearest - v); // 結果の座標差分
                var r = key.shape.Evaluate(time);

                // 法線をミックス
                if (0f < key.addNormal && 0f <= time && time <= 1f)
                {
                    var n = -normals[j].normalized; // 法線の逆方向
                    var nt = n - Vector3.Project(n, w.normalized); // nのwに対して垂直な成分
                    w += nt * key.addNormal * (1f - r);
                }

                vertices[j] = w;
                toBeRemoved[j] = r < key.removeThreshold;
            }
            if (key.removeThreshold < 1.0f) result.AddBlendShapeFrame(key.name, 100f, vertices, null, null);

            if (0.0f < key.removeThreshold)
            {
                // ポリゴン削除
                var srcTriangles = result.triangles;
                var k = 0;
                var n = srcTriangles.Length;
                var triangles = new int[n];
                for (var i = 0; i < n; i += 3)
                {
                    var v0 = srcTriangles[i];
                    var v1 = srcTriangles[i+1];
                    var v2 = srcTriangles[i+2];
                    if (toBeRemoved[v0] && toBeRemoved[v1] && toBeRemoved[v2]) continue;
                    triangles[k++] = v0;
                    triangles[k++] = v1;
                    triangles[k++] = v2;
                }
                if (k < n) result.triangles = triangles.Take(k).ToArray();
            }
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
