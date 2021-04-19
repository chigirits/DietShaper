using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chigiri.DietShaper.Editor
{
    public class ShaperImpl
    {

        public static void Process(DietShaper p)
        {
            var targetScale = p.targetRenderer.transform.localScale;
            if (false) p.targetRenderer.transform.localScale = Vector3.one;
            var resultMesh = DoProcess(p);
            if (false) p.targetRenderer.transform.localScale = targetScale;

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

        static void AddBoneKey(DietShaper p, ShapeKey key, ShapeKey exceptKey, Vector3[] posed, Vector3[] normals, Mesh result, List<Vector3> debugPoints)
        {
            if (!key.enable) return;
            key.shape.preWrapMode = WrapMode.ClampForever;
            key.shape.postWrapMode = WrapMode.ClampForever;
            var vertices = new Vector3[p.sourceMesh.vertexCount];
            var w2l = p.targetRenderer.worldToLocalMatrix;
            var avs = p.avatarRoot.transform.lossyScale;
            var l2ws = p.targetRenderer.localToWorldMatrix.lossyScale;
            var rescale = new Vector3(l2ws.x/avs.x, l2ws.y/avs.y, l2ws.z/avs.z);
            // FIXME: SkinnedMeshRenderer の元々の（FBX内での）Scale を知りたい
            var rts = p.targetRenderer.rootBone.lossyScale;
            var rtsInv = new Vector3(1f/rts.x, 1f/rts.y, 1f/rts.z);
            var resolver = new NearestPointResolver(p.avatarRoot, key, p.isGenericMode);
            var rNormal = key.addNormal / avs.magnitude;
            var toBeRemoved = new bool[p.sourceMesh.vertexCount];
            for (var j = 0; j < p.sourceMesh.vertexCount; j++)
            {
                var v = posed[j];
                var (nearest, time, distance) = resolver.Resolve(v);
                var radius = key.radius; // Mathf.Lerp(key.startRadius, key.endRadius, time);
                if (radius < distance) continue;
                debugPoints.Add(nearest);
                var w = w2l.MultiplyVector(nearest - v); // 結果の座標差分
                if (p.adjustScale) w.Scale(rescale);
                var r = key.shape.Evaluate(time);
                r = 1 - (1-r) * key.weight;

                // 法線をミックス
                if (0f < rNormal && 0f <= time && time <= 1f)
                {
                    var n = -normals[j].normalized; // 法線の逆方向
                    var nt = n - Vector3.Project(n, w.normalized); // nのwに対して垂直な成分
                    w += Vector3.Scale(nt * rNormal * (1f - r), rtsInv);
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

            var debugPoints = new List<Vector3>();
            foreach (var key in p.shapeKeys)
            {
                AddBoneKey(p, key, null, posed, p.sourceMesh.normals, result, debugPoints);
            }

            // // 結果を点群でデバッグ表示
            // Helper.CreateSparseCubes("PosedDebugRoot", debugPoints, 20);

            return result;
        }

    }
}
