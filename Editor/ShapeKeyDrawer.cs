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
            var bodyLines = property.FindPropertyRelative("bodyLines");
            var isLoaded = 0 < bodyLines.arraySize;
            var h = (height + spacing) * (isLoaded ? 8 : 1);
            for (var i = 0; i < bodyLines.arraySize; i++)
            {
                h += EditorGUI.GetPropertyHeight(bodyLines.GetArrayElementAtIndex(i)) + spacing;
            }
            return h - spacing;
        }

        public static void CopyProperties(SerializedProperty property, ShapeKey value)
        {
            property.FindPropertyRelative("enable").boolValue = value.enable;
            property.FindPropertyRelative("name").stringValue = value.name;
            property.FindPropertyRelative("radius").floatValue = value.radius;
            property.FindPropertyRelative("addNormal").floatValue = value.addNormal;
            property.FindPropertyRelative("isLeaf").boolValue = value.isLeaf;
            property.FindPropertyRelative("shape").animationCurveValue = value.shape;
            property.FindPropertyRelative("gizmoColor").colorValue = value.gizmoColor;
            var bodyLines = property.FindPropertyRelative("bodyLines");
            bodyLines.arraySize = value.bodyLines.Count;
            for (var i = 0; i < bodyLines.arraySize; i++)
            {
                var bodyLine = bodyLines.GetArrayElementAtIndex(i);
                bodyLine.FindPropertyRelative("_index").intValue = i;
                BodyLineDrawer.CopyProperties(bodyLine, value.bodyLines[i]);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = new Rect(position.x, position.y, position.width, height);

            var r = new Rect(rect.x, rect.y, rect.height, rect.height);
            var bodyLines = property.FindPropertyRelative("bodyLines");
            var isLoaded = 0 < bodyLines.arraySize;
            var name = property.FindPropertyRelative("name").stringValue;
            if (name == "") name = "(empty)";
            var enable = property.FindPropertyRelative("enable");
            if (isLoaded)
                EditorGUI.PropertyField(rect, enable, new GUIContent(name, ""));
            else
                EditorGUI.LabelField(rect, new GUIContent(name, ""));
            rect.y += height + spacing;

            if (!isLoaded) return;

            EditorGUI.BeginDisabledGroup(!enable.boolValue);
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("name"), new GUIContent("Name", ""));
                rect.y += height + spacing;
                EditorGUI.Slider(rect, property.FindPropertyRelative("radius"), 0f, 1f, new GUIContent("Radius", ""));
                rect.y += height + spacing;
                EditorGUI.Slider(rect, property.FindPropertyRelative("addNormal"), 0f, 0.1f, new GUIContent("Add Normal", "法線を元にした成分の影響力（単位：メートル）。"));
                rect.y += height + spacing;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("isLeaf"), new GUIContent("Is Leaf", ""));
                rect.y += height + spacing;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("shape"), new GUIContent("Shape", ""));
                rect.y += height + spacing;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("gizmoColor"), new GUIContent("Gizmo Color", ""));
                rect.y += height + spacing;

                for (var i = 0; i < bodyLines.arraySize; i++)
                {
                    var bodyLine = bodyLines.GetArrayElementAtIndex(i);
                    bodyLine.FindPropertyRelative("_index").intValue = i;
                    EditorGUI.PropertyField(rect, bodyLine);
                    rect.y += EditorGUI.GetPropertyHeight(bodyLine) + spacing;
                }

                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
        }

    }

}
