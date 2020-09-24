using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace Chigiri.DietShaper.Editor
{

    [CustomEditor(typeof(DietShaper))]
    public class DietShaperEditor : UnityEditor.Editor
    {

        ReorderableList reorderableList;
        List<string> blendShapes;

        [MenuItem("Chigiri/Create DietShaper")]
        public static void CreateDietShaper()
        {
            var path = AssetDatabase.GUIDToAssetPath("12c469a9f19e32744a18d7e7eefef715");
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            instance.transform.SetAsLastSibling();
            Selection.activeGameObject = instance;
            Undo.RegisterCreatedObjectUndo(instance, "Create DietShaper");
        }

        private void OnEnable()
        {
        }

        DietShaper self
        {
            get { return target as DietShaper; }
        }

        SerializedProperty avatarRoot
        {
            get { return serializedObject.FindProperty("avatarRoot"); }
        }

        SerializedProperty targetRenderer
        {
            get { return serializedObject.FindProperty("targetRenderer"); }
        }

        SerializedProperty sourceMesh
        {
            get { return serializedObject.FindProperty("sourceMesh"); }
        }

        SerializedProperty alwaysShowGizmo
        {
            get { return serializedObject.FindProperty("alwaysShowGizmo"); }
        }

        SerializedProperty shapeKeys
        {
            get { return serializedObject.FindProperty("shapeKeys"); }
        }

        // Revert Target ボタンを有効にするときtrue
        bool isRevertTargetEnable
        {
            get
            {
                return targetRenderer.objectReferenceValue != null &&
                    sourceMesh.objectReferenceValue != null;
            }
        }

        public override void OnInspectorGUI()
        {
            // 操作前の値を一部保持（比較用）
            var prevTargetRenderer = self.targetRenderer;
            var prevSourceMesh = self.sourceMesh;

            // 描画
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            {
                // UI描画

                EditorGUILayout.PropertyField(avatarRoot, new GUIContent("Avatar Root", "操作対象のアバターのルートオブジェクト"));
                EditorGUILayout.PropertyField(targetRenderer, new GUIContent("Target", "操作対象のSkinnedMeshRenderer"));
                EditorGUILayout.PropertyField(sourceMesh, new GUIContent("Source Mesh", "オリジナルのメッシュ"));
                EditorGUILayout.PropertyField(alwaysShowGizmo, new GUIContent("Always Show Gizmo", "非選択状態でもギズモを表示"));

                for (var i = 0; i < shapeKeys.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(shapeKeys.GetArrayElementAtIndex(i));
                }

                // エラー表示
                var error = Validate();
                if (error != "")
                {
                    EditorGUILayout.HelpBox(Helper.Chomp(error), MessageType.Error, true);
                }

                // Revert Target に関する注意
                if (isRevertTargetEnable)
                {
                    EditorGUILayout.HelpBox("Undo 時にメッシュが消えた場合は Revert Target ボタンを押してください。", MessageType.Info, true);
                }

                EditorGUILayout.BeginHorizontal();
                {
                    // Process And Save As... ボタン
                    EditorGUI.BeginDisabledGroup(error != "");
                    if (GUILayout.Button(new GUIContent("Process And Save As...", "新しいメッシュを生成し、保存ダイアログを表示します。")))
                    {
                        ShaperImpl.Process(self);
                    }
                    EditorGUI.EndDisabledGroup();

                    // Revert Target ボタン
                    EditorGUI.BeginDisabledGroup(!isRevertTargetEnable);
                    if (GUILayout.Button(new GUIContent("Revert Target", "Target の SkinnedMeshRenderer にアタッチされていたメッシュを元に戻します。"))) RevertTarget();
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                // 何らかの操作があったときに必要な処理
                // noop
            }

            // Target を変更したときに Source Mesh が空なら自動設定
            if (prevTargetRenderer != self.targetRenderer && self.targetRenderer != null && self.sourceMesh == null)
            {
                self.sourceMesh = self.targetRenderer.sharedMesh;
            }
        }

        string Validate()
        {
            if (avatarRoot.objectReferenceValue == null)
            {
                return "Avatar Root を指定してください";
            }
            if (!(avatarRoot.objectReferenceValue as Animator).isHuman)
            {
                return "Avatar Root に指定されたオブジェクトは Humanoid である必要があります";
            }
            if (targetRenderer.objectReferenceValue == null)
            {
                return "Target を指定してください";
            }
            if (sourceMesh.objectReferenceValue == null || self.sourceMesh == null)
            {
                return "Source Mesh を指定してください";
            }
            return "";
        }

        void RevertTarget()
        {
            Undo.RecordObject(self.targetRenderer, "Revert Target (MeshHoleShrinker)");
            self.targetRenderer.sharedMesh = self.sourceMesh;
        }

    }
}
