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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var isOpen = property.FindPropertyRelative("_isOpen");
            if (!isOpen.boolValue) return height;
            var bones = property.FindPropertyRelative("bones");
            return (height + spacing) * (4 + bones.arraySize) - spacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = new Rect(position.x, position.y, position.width, height);

            var index = property.FindPropertyRelative("_index");
            var isOpen = property.FindPropertyRelative("_isOpen");
            isOpen.boolValue = EditorGUI.Foldout(rect, isOpen.boolValue, $"Body Lines [{index.intValue}]");
            rect.y += height + spacing;

            if (isOpen.boolValue)
            {
                EditorGUI.indentLevel++;
                var bones = property.FindPropertyRelative("bones");
                for (var i=0; i<bones.arraySize; i++)
                {
                    var bone = bones.GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(rect, bone, new GUIContent($"Bones [{i}]", ""));
                    rect.y += height + spacing;
                }
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("isLeaf"), new GUIContent("Is Leaf", ""));
                rect.y += height + spacing;
                EditorGUI.Slider(rect, property.FindPropertyRelative("startMargin"), 0f, 0.5f, new GUIContent("Start Margin", ""));
                rect.y += height + spacing;
                EditorGUI.Slider(rect, property.FindPropertyRelative("endMargin"), 0f, 0.5f, new GUIContent("End Margin", ""));
                rect.y += height + spacing;
                EditorGUI.indentLevel--;
            }
        }

    }

}
