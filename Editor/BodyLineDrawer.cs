using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Chigiri.DietShaper.Editor
{

    [CustomPropertyDrawer(typeof(BodyLine))]
    public class BodyLineDrawer : PropertyDrawer
    {

        static float height = EditorGUIUtility.singleLineHeight;
        static float spacing = EditorGUIUtility.standardVerticalSpacing;

        public static void CopyProperties(SerializedProperty property, BodyLine value)
        {
            var bones = property.FindPropertyRelative("bones");
            bones.arraySize = value.bones.Count;
            for (var i=0; i<bones.arraySize; i++)
            {
                var boneIndex = Array.IndexOf(Enum.GetValues(typeof(HumanBodyBones)), value.bones[i]);
                bones.GetArrayElementAtIndex(i).enumValueIndex = boneIndex;
            }

            var genericBones = property.FindPropertyRelative("genericBones");
            genericBones.arraySize = value.genericBones.Count;
            for (var i=0; i<genericBones.arraySize; i++)
            {
                var genericBone = genericBones.GetArrayElementAtIndex(i);
                genericBone.objectReferenceValue = value.genericBones[i];
            }

            var xSignRangeIndex = Array.IndexOf(Enum.GetValues(typeof(SignRange)), value.xSignRange);
            property.FindPropertyRelative("xSignRange").enumValueIndex = xSignRangeIndex;
        }

        public static void Normalize(SerializedProperty property)
        {
            var bones = property.FindPropertyRelative("bones");
            var genericBones = property.FindPropertyRelative("genericBones");
            if (!genericBones.isArray || genericBones.arraySize < bones.arraySize) genericBones.arraySize = bones.arraySize;
            // TODO: オブジェクト名からボーンを自動補完
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var bones = property.FindPropertyRelative("bones");
            var h = (height + spacing) * (2 + 1 * bones.arraySize);
            return h - spacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = new Rect(position.x, position.y, position.width, height);
            Normalize(property);

            var index = property.FindPropertyRelative("_index");
            var isGenericMode = property.FindPropertyRelative("_isGenericMode");
            EditorGUI.LabelField(rect, new GUIContent(
                $"Body Lines [{index.intValue}]",
                $"{index.intValue}番目の処理対象ボディライン。" +
                "左右の腕など、複数のボーンを別々に処理した結果を1つのシェイプキーにするときは2つ以上のボディラインを持ちます。" +
                "ボディラインの数は変更できません。通常はプリセットの設定を変更しないでください。"
            ));
            rect.y += height + spacing;

            {
                EditorGUI.indentLevel++;
                if (isGenericMode.boolValue)
                {
                    var genericBones = property.FindPropertyRelative("genericBones");
                    for (var i=0; i<genericBones.arraySize; i++)
                    {
                        var genericBone = genericBones.GetArrayElementAtIndex(i);
                        EditorGUI.PropertyField(rect, genericBone, new GUIContent(
                            $"Bones [{i}]",
                            "これらのボーンをつないだ線分または折れ線（ボディライン）に向かって周囲の頂点が吸着するように変形されます。" +
                            "ボーンの数は変更できません。Generic ModeではHumanoid以外（服など）のボーンを直接指定できます。"
                        ));
                        rect.y += height + spacing;
                    }
                }
                else
                {
                    var bones = property.FindPropertyRelative("bones");
                    for (var i=0; i<bones.arraySize; i++)
                    {
                        var bone = bones.GetArrayElementAtIndex(i);
                        EditorGUI.PropertyField(rect, bone, new GUIContent(
                            $"Bones [{i}]",
                            $"{i}番目のボーン。" +
                            "これらのボーンをつないだ線分または折れ線（ボディライン）に向かって周囲の頂点が吸着するように変形されます。" +
                            "ボーンの数は変更できません。通常はプリセットの設定を変更しないでください。"
                        ));
                        rect.y += height + spacing;
                    }
                }
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("xSignRange"), new GUIContent("X Sign Range", "処理対象に含める頂点のX座標の符号範囲。脚など、左右で円筒が重なりやすい部分の排他処理に用います。通常はプリセットの設定を変更しないでください。"));
                rect.y += height + spacing;
                EditorGUI.indentLevel--;
            }
        }

    }

}
