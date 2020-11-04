using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Helper
{

    public static Rect[] SplitRect(Rect rect, bool vertical, params float[] widths)
    {
        var result = new Rect[widths.Length];
        var sizes = new float[widths.Length];

        var fixedTotal = 0f;
        var expandWeights = 0f;
        for (var i = 0; i < widths.Length; i++)
        {
            var w = widths[i];
            if (w < 0)
            {
                expandWeights -= w;
            }
            else
            {
                sizes[i] = w;
                fixedTotal += w;
            }
        }

        var expandTotal = (vertical ? rect.height : rect.width) - fixedTotal;
        for (var i = 0; i < widths.Length; i++)
        {
            var w = widths[i];
            if (w < 0) sizes[i] = expandTotal * (-w) / expandWeights;
        }

        var pos = vertical ? rect.y : rect.x;
        for (var i = 0; i < sizes.Length; i++)
        {
            var w = sizes[i];
            result[i] = vertical ? new Rect(rect.x, pos, rect.width, w) : new Rect(pos, rect.y, w, rect.height);
            pos += w;
        }

        return result;
    }

    public static string Chomp(string s)
    {
        if (s.EndsWith("\n")) return s.Substring(0, s.Length - 1);
        return s;
    }

    public static string SanitizeFileName(string name)
    {
        var reg = new Regex("[\\/:\\*\\?<>\\|\\\"]");
        return reg.Replace(name, "_");
    }

    public static Vector3[] AddVector3(Vector3[] src0, Vector3[] src1, float scale)
    {
        var result = new Vector3[src0.Length];
        for (int i = 0; i < src0.Length; i++) result[i] = src0[i] + src1[i] * scale;
        return result;
    }

    public static float Vector2Cross(Vector2 lhs, Vector2 rhs)
    {
        return lhs.x * rhs.y - lhs.y * rhs.x;
    }

    // From https://forum.unity.com/threads/bakemesh-scales-wrong.442212/#post-2860559
    public static Vector3[] GetPosedVertices(SkinnedMeshRenderer skin, Mesh sharedMesh)
    {
        float MIN_VALUE = 0.00001f;

        Vector3[] vertices = sharedMesh.vertices;
        Matrix4x4[] bindposes = sharedMesh.bindposes;
        BoneWeight[] boneWeights = sharedMesh.boneWeights;
        Transform[] bones = skin.bones;
        Vector3[] newVert = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            BoneWeight bw = boneWeights[i];

            if (Mathf.Abs(bw.weight0) > MIN_VALUE)
            {
                var p = bindposes[bw.boneIndex0].MultiplyPoint3x4(vertices[i]);
                var q = bones[bw.boneIndex0].transform.localToWorldMatrix.MultiplyPoint3x4(p);
                newVert[i] += q * bw.weight0;
            }
            if (Mathf.Abs(bw.weight1) > MIN_VALUE)
            {
                var p = bindposes[bw.boneIndex1].MultiplyPoint3x4(vertices[i]);
                var q = bones[bw.boneIndex1].transform.localToWorldMatrix.MultiplyPoint3x4(p);
                newVert[i] += q * bw.weight1;
            }
            if (Mathf.Abs(bw.weight2) > MIN_VALUE)
            {
                var p = bindposes[bw.boneIndex2].MultiplyPoint3x4(vertices[i]);
                var q = bones[bw.boneIndex2].transform.localToWorldMatrix.MultiplyPoint3x4(p);
                newVert[i] += q * bw.weight2;
            }
            if (Mathf.Abs(bw.weight3) > MIN_VALUE)
            {
                var p = bindposes[bw.boneIndex3].MultiplyPoint3x4(vertices[i]);
                var q = bones[bw.boneIndex3].transform.localToWorldMatrix.MultiplyPoint3x4(p);
                newVert[i] += q * bw.weight3;
            }

        }

        return newVert;
    }

}
