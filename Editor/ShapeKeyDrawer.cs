using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Chigiri.DietShaper.Editor
{

    [CustomPropertyDrawer(typeof(ShapeKey))]
    public class ShapeKeyDrawer : PropertyDrawer
    {

        static float height = EditorGUIUtility.singleLineHeight;
        static float spacing = EditorGUIUtility.standardVerticalSpacing;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var isOpen = property.FindPropertyRelative("_isOpen");
            if (!isOpen.boolValue) return height;
            var bodyLines = property.FindPropertyRelative("bodyLines");
            var h = (height + spacing) * 5;
            for (var i = 0; i < bodyLines.arraySize; i++)
            {
                h += EditorGUI.GetPropertyHeight(bodyLines.GetArrayElementAtIndex(i)) + spacing;
            }
            return h - spacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel++;
            var rect = new Rect(position.x, position.y, position.width, height);
            var isOpen = property.FindPropertyRelative("_isOpen");

            var r = new Rect(rect.x, rect.y, rect.height, rect.height);
            var name = property.FindPropertyRelative("name").stringValue;
            if (name == "") name = "(unnamed)";
            isOpen.boolValue = EditorGUI.Foldout(r, isOpen.boolValue, "");
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("enable"), new GUIContent(name, ""));
            rect.y += height + spacing;

            if (isOpen.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("name"), new GUIContent("Name", ""));
                rect.y += height + spacing;
                EditorGUI.Slider(rect, property.FindPropertyRelative("radius"), 0f, 1f, new GUIContent("Radius", ""));
                rect.y += height + spacing;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("shape"), new GUIContent("Shape", ""));
                rect.y += height + spacing;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("gizmoColor"), new GUIContent("Gizmo Color", ""));
                rect.y += height + spacing;

                var bodyLines = property.FindPropertyRelative("bodyLines");
                for (var i = 0; i < bodyLines.arraySize; i++)
                {
                    var bodyLine = bodyLines.GetArrayElementAtIndex(i);
                    bodyLine.FindPropertyRelative("_index").intValue = i;
                    EditorGUI.PropertyField(rect, bodyLine);
                    rect.y += EditorGUI.GetPropertyHeight(bodyLine) + spacing;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }

    }

}
