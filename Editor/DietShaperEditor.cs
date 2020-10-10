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
        List<string> presets;

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

        static float lineHeight
        {
            get
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        static float linePitch
        {
            get
            {
                return lineHeight + 2f;
            }
        }

        // リスト要素の高さ
        float ElementHeightCallback(int i)
        {
            return linePitch;
        }

        // リスト要素を描画
        void DrawElementCallback(Rect rect, int i, bool isActive, bool isFocused)
        {
            var orgLabelWidth = EditorGUIUtility.labelWidth;

            var shapeKey = shapeKeys.GetArrayElementAtIndex(i);
            var r = new Rect(rect.x, rect.y, rect.width, lineHeight);

            // 各フィールドを描画
            EditorGUIUtility.labelWidth = r.width - r.height * 1.5f;
            var enable = shapeKey.FindPropertyRelative("enable");
            var name = shapeKey.FindPropertyRelative("name").stringValue;
            if (name == "") name = "(empty)";
            var isLoaded = 0 < shapeKey.FindPropertyRelative("bodyLines").arraySize;
            if (isLoaded)
                EditorGUI.PropertyField(r, enable, new GUIContent(name, ""));
            else
                EditorGUI.LabelField(r, new GUIContent(name, ""));
            r.y += linePitch;

            EditorGUIUtility.labelWidth = orgLabelWidth;
        }

        // リスト描画の準備
        void PrepareReordableList()
        {
            if (reorderableList != null) return;

            reorderableList = new ReorderableList(
                elements: self.shapeKeys,
                elementType: typeof(ShapeKey),
                draggable: true,
                displayHeader: true,
                displayAddButton: true,
                displayRemoveButton: true
            );

            // reorderableList.drawElementBackgroundCallback = (Rect rect, int i, bool isActive, bool isFocused) => { };

            reorderableList.drawElementCallback = DrawElementCallback;

            // reorderableList.drawFooterCallback = rect => { };
            reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Shape Keys");
            reorderableList.elementHeightCallback = ElementHeightCallback;
            reorderableList.onAddCallback = list =>
            {
                // Shape Key を追加
                var n = shapeKeys.arraySize;
                shapeKeys.InsertArrayElementAtIndex(n);
            };
            // reorderableList.onAddDropdownCallback = (rect, list) => Debug.Log("onAddDropdown");
            reorderableList.onCanAddCallback = list => 0 < shapeKeys.arraySize;
            // reorderableList.onCanRemoveCallback = list => true;
            // reorderableList.onChangedCallback = list => Debug.Log("onChanged");
            // reorderableList.onMouseUpCallback = list => { };
            reorderableList.onRemoveCallback = list =>
            {
                shapeKeys.DeleteArrayElementAtIndex(list.index);
                if (shapeKeys.arraySize <= list.index) list.index--;
            };
            reorderableList.onReorderCallback = list => Debug.Log("onReorder");
            reorderableList.onSelectCallback = list => Debug.Log("onSelect");
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
                // リスト描画の準備
                PrepareReordableList();

                // UI描画

                EditorGUILayout.PropertyField(avatarRoot, new GUIContent("Avatar Root", "操作対象のアバターのルートオブジェクト"));
                EditorGUILayout.PropertyField(targetRenderer, new GUIContent("Target", "操作対象のSkinnedMeshRenderer"));
                EditorGUILayout.PropertyField(sourceMesh, new GUIContent("Source Mesh", "オリジナルのメッシュ"));
                EditorGUILayout.PropertyField(alwaysShowGizmo, new GUIContent("Always Show Gizmo", "非選択状態でもギズモを表示"));
                reorderableList.DoLayoutList();

                var presetIndex = EditorGUILayout.Popup("Add Shape Key From Preset", -1, ShapeKey.presetKeys);
                if (0 <= presetIndex)
                {
                    var n = shapeKeys.arraySize;
                    shapeKeys.arraySize++;
                    var presetKey = ShapeKey.presetKeys[presetIndex];
                    ShapeKeyDrawer.CopyProperties(shapeKeys.GetArrayElementAtIndex(n), ShapeKey.presets[presetKey]);
                    reorderableList.index = n;
                }

                if (0 <= reorderableList.index)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.PropertyField(shapeKeys.GetArrayElementAtIndex(reorderableList.index));
                    EditorGUILayout.EndVertical();
                }

                // エラー表示
                EditorGUILayout.Space();
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
            if (shapeKeys.arraySize == 0)
            {
                return "Shape Keys を1つ以上作成してください";
            }

            var names = new Dictionary<string, bool>();
            for (var i = 0; i < shapeKeys.arraySize; i++)
            {
                var shapeKey = shapeKeys.GetArrayElementAtIndex(i);
                var enable = shapeKey.FindPropertyRelative("enable").boolValue;
                if (!enable) continue;
                var name = shapeKey.FindPropertyRelative("name").stringValue;
                if (names.ContainsKey(name))
                {
                    return $"シェイプキー名 {name} が重複しています。異なる名前を指定してください";
                }
                names[name] = true;
                for (var j = 0; j < self.sourceMesh.blendShapeCount; j++)
                {
                    if (name == self.sourceMesh.GetBlendShapeName(j))
                    {
                        return $"Source Mesh には {name} というシェイプキーが既に定義されています。 " +
                            "名前を変更するか、または処理済みのメッシュが指定されていないかを確認してください";
                    }
                }
            }

            return "";
        }

        void RevertTarget()
        {
            Undo.RecordObject(self.targetRenderer, "Revert Target (DietShaper)");
            self.targetRenderer.sharedMesh = self.sourceMesh;
        }

    }
}
