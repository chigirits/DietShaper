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
            var h = (height + spacing) * (isLoaded ? 10 : 1);
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
            property.FindPropertyRelative("startMargin").floatValue = value.startMargin;
            property.FindPropertyRelative("endMargin").floatValue = value.endMargin;
            property.FindPropertyRelative("isLeaf").boolValue = value.isLeaf;
            property.FindPropertyRelative("shape").animationCurveValue = value.shape;
            property.FindPropertyRelative("addNormal").floatValue = value.addNormal;
            property.FindPropertyRelative("removeThreshold").floatValue = value.removeThreshold;
            property.FindPropertyRelative("gizmoColor").colorValue = value.gizmoColor;
            var bodyLines = property.FindPropertyRelative("bodyLines");
            bodyLines.arraySize = value.bodyLines.Count;
            for (var i = 0; i < bodyLines.arraySize; i++)
            {
                var bodyLine = bodyLines.GetArrayElementAtIndex(i);
                bodyLine.FindPropertyRelative("_index").intValue = i;
                bodyLine.FindPropertyRelative("_isGenericMode").boolValue = value._isGenericMode;
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
                EditorGUI.PropertyField(rect, enable, new GUIContent(name, "チェックされているときのみ、このシェイプキーが作成されます。"));
            else
                EditorGUI.LabelField(rect, new GUIContent(name));
            rect.y += height + spacing;

            if (!isLoaded) return;

            var isLeafEnable = 0 < bodyLines.arraySize && bodyLines.GetArrayElementAtIndex(0).FindPropertyRelative("bones").arraySize == 2;
            EditorGUI.BeginDisabledGroup(!enable.boolValue);
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("name"), new GUIContent("Name", "作成するシェイプキーの名前。"));
                rect.y += height + spacing;
                EditorGUI.Slider(rect, property.FindPropertyRelative("radius"), 0f, 1f, new GUIContent("Radius", "処理対象に含める範囲を表す円筒の半径。大きすぎると無関係な頂点まで変形されてしまうため、適度な値を設定する必要があります。"));
                rect.y += height + spacing;
                EditorGUI.Slider(rect, property.FindPropertyRelative("startMargin"), 0f, 0.499f, new GUIContent("Start Margin", "変形範囲を狭くするとき、開始ボーンからの距離を比率で指定します。"));
                rect.y += height + spacing;
                EditorGUI.Slider(rect, property.FindPropertyRelative("endMargin"), 0f, 0.499f, new GUIContent("End Margin", "変形範囲を狭くするとき、終端ボーンからの距離を比率で指定します。"));
                rect.y += height + spacing;
                EditorGUI.BeginDisabledGroup(!isLeafEnable);
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("isLeaf"), new GUIContent("Is Leaf", "手足の先など、終端点を超えてスキンの先端まですべての頂点を処理対象に含めるときにチェックします。通常のボーンに沿うような変形とは異なり、開始点に向かって均等に縮められます。"));
                rect.y += height + spacing;
                EditorGUI.EndDisabledGroup();
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("shape"), new GUIContent("Shape", "変形の形状。開始点を time=0（左端）、終端点を time=1（右端）とし、縦軸にボーンへの吸着強度（0=最大、1=変形なし）を指定します。"));
                rect.y += height + spacing;
                EditorGUI.Slider(rect, property.FindPropertyRelative("addNormal"), 0f, 0.1f, new GUIContent("Add Normal", "法線を元にした成分の影響力（単位：メートル）。通常は 0 にしてください。Shoulder プリセットで、わきの下をボーンとは垂直な方向に移動するために用います。"));
                rect.y += height + spacing;
                EditorGUI.Slider(rect, property.FindPropertyRelative("removeThreshold"), 0f, 1.0f, new GUIContent("Remove Threshold", "ポリゴンを削除するしきい値。Shape がこの値未満になる範囲のポリゴンを削除します。通常は 0 に設定してください（ポリゴンは一つも削除されません）。1 にした場合、シェイプキーは作成されません。"));
                rect.y += height + spacing;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("gizmoColor"), new GUIContent("Gizmo Color", "ギズモの表示色。処理内容への影響はありません。"));
                rect.y += height + spacing;

                var isGenericMode = property.FindPropertyRelative("_isGenericMode");
                for (var i = 0; i < bodyLines.arraySize; i++)
                {
                    var bodyLine = bodyLines.GetArrayElementAtIndex(i);
                    bodyLine.FindPropertyRelative("_index").intValue = i;
                    bodyLine.FindPropertyRelative("_isGenericMode").boolValue = isGenericMode.boolValue;
                    EditorGUI.PropertyField(rect, bodyLine);
                    rect.y += EditorGUI.GetPropertyHeight(bodyLine) + spacing;
                }

                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
        }

    }

}
