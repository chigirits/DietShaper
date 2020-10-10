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
            var xSignRangeIndex = Array.IndexOf(Enum.GetValues(typeof(SignRange)), value.xSignRange);
            property.FindPropertyRelative("xSignRange").enumValueIndex = xSignRangeIndex;
            property.FindPropertyRelative("startMargin").floatValue = value.startMargin;
            property.FindPropertyRelative("endMargin").floatValue = value.endMargin;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var bones = property.FindPropertyRelative("bones");
            return (height + spacing) * (4 + bones.arraySize) - spacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = new Rect(position.x, position.y, position.width, height);

            var index = property.FindPropertyRelative("_index");
            EditorGUI.LabelField(rect, $"Body Lines [{index.intValue}]");
            rect.y += height + spacing;

            {
                EditorGUI.indentLevel++;
                var bones = property.FindPropertyRelative("bones");
                for (var i=0; i<bones.arraySize; i++)
                {
                    var bone = bones.GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(rect, bone, new GUIContent($"Bones [{i}]", ""));
                    rect.y += height + spacing;
                }
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("xSignRange"), new GUIContent("X Sign Range", ""));
                rect.y += height + spacing;
                EditorGUI.Slider(rect, property.FindPropertyRelative("startMargin"), 0f, 0.499f, new GUIContent("Start Margin", ""));
                rect.y += height + spacing;
                EditorGUI.Slider(rect, property.FindPropertyRelative("endMargin"), 0f, 0.499f, new GUIContent("End Margin", ""));
                rect.y += height + spacing;
                EditorGUI.indentLevel--;
            }
        }

    }

}
