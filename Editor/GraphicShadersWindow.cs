using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace LuviKunG.GraphicShaders
{
    public sealed class GraphicShadersWindow : EditorWindow
    {
        private const string GRAPHIC_SETTINGS_ASSET_PATH = "ProjectSettings/GraphicsSettings.asset";

        private static readonly GUIContent CONTENT_LIST_HEADER = new GUIContent("Always Include Shaders");
        private static readonly GUIContent CONTENT_BUTTON_REMOVE_NULL_REFERENCE = new GUIContent("Remove Null Reference");
        private static readonly GUIContent CONTENT_BUTTON_REMOVE_DUPLICATION = new GUIContent("Remove Duplication");
        private static readonly GUIContent CONTENT_BUTTON_SORT_BY_NAME = new GUIContent("Sort by Name");

        private SerializedObject graphicsManager;
        private SerializedProperty alwaysIncludedShaders;
        private ReorderableList listShader;

        public static GraphicShadersWindow OpenWindow()
        {
            var window = GetWindow<GraphicShadersWindow>(false, "Graphic Shaders", true);
            window.Show();
            return window;
        }

        private void OnEnable()
        {
            graphicsManager = new SerializedObject(AssetDatabase.LoadAssetAtPath(GRAPHIC_SETTINGS_ASSET_PATH, typeof(UnityEngine.Object)));
            alwaysIncludedShaders = graphicsManager.FindProperty("m_AlwaysIncludedShaders");

            listShader = new ReorderableList(graphicsManager, alwaysIncludedShaders, true, true, true, true);
            listShader.drawHeaderCallback = DrawHeader;
            listShader.drawElementCallback = DrawElement;
            listShader.elementHeightCallback = ElementHeight;
            listShader.onAddCallback = OnAdd;
            listShader.onRemoveCallback = OnRemove;
            listShader.onReorderCallback = OnReorder;
            //local function.
            void DrawHeader(Rect rect)
            {
                EditorGUI.LabelField(rect, CONTENT_LIST_HEADER);
            }
            void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                var element = alwaysIncludedShaders.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(rect, element, typeof(Shader), GUIContent.none);
            }
            float ElementHeight(int index)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            void OnAdd(ReorderableList list)
            {
                if (list.index < 0)
                    alwaysIncludedShaders.InsertArrayElementAtIndex(alwaysIncludedShaders.arraySize);
                else
                    alwaysIncludedShaders.InsertArrayElementAtIndex(list.index);
                graphicsManager.ApplyModifiedProperties();
            }
            void OnRemove(ReorderableList list)
            {
                if (list.index < 0)
                    return;
                var element = alwaysIncludedShaders.GetArrayElementAtIndex(list.index);
                if (element.objectReferenceValue != null)
                    alwaysIncludedShaders.DeleteArrayElementAtIndex(list.index);
                alwaysIncludedShaders.DeleteArrayElementAtIndex(list.index);
                graphicsManager.ApplyModifiedProperties();
            }
            void OnReorder(ReorderableList list)
            {
                graphicsManager.ApplyModifiedProperties();
            }
        }

        private void OnGUI()
        {
            using (var horizontalScope = new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button(CONTENT_BUTTON_REMOVE_NULL_REFERENCE, EditorStyles.toolbarButton))
                    RemoveNullReference();
                if (GUILayout.Button(CONTENT_BUTTON_REMOVE_DUPLICATION, EditorStyles.toolbarButton))
                    RemoveDuplicationShaders();
                if (GUILayout.Button(CONTENT_BUTTON_SORT_BY_NAME, EditorStyles.toolbarButton))
                    SortByName();
                GUILayout.FlexibleSpace();

            }
            using (var verticalScope = new EditorGUILayout.VerticalScope())
                listShader.DoLayoutList();
        }

        private void RemoveNullReference()
        {
            for (int i = 0; i < alwaysIncludedShaders.arraySize; i++)
            {
                var element = alwaysIncludedShaders.GetArrayElementAtIndex(i);
                if (element.objectReferenceValue == null)
                    alwaysIncludedShaders.DeleteArrayElementAtIndex(i--);
            }
            graphicsManager.ApplyModifiedProperties();
        }

        private void RemoveDuplicationShaders()
        {
            for (int i = 0; i < alwaysIncludedShaders.arraySize; i++)
            {
                var element = alwaysIncludedShaders.GetArrayElementAtIndex(i);
                for (int j = i + 1; j < alwaysIncludedShaders.arraySize; j++)
                {
                    var elementComparer = alwaysIncludedShaders.GetArrayElementAtIndex(j);
                    if (element.objectReferenceValue == elementComparer.objectReferenceValue)
                    {
                        if (elementComparer.objectReferenceValue != null)
                            alwaysIncludedShaders.DeleteArrayElementAtIndex(j);
                        alwaysIncludedShaders.DeleteArrayElementAtIndex(j--);
                    }
                }
            }
            graphicsManager.ApplyModifiedProperties();
        }

        private void SortByName()
        {
            var list = new List<UnityEngine.Object>();
            for (int i = 0; i < alwaysIncludedShaders.arraySize; i++)
            {
                var element = alwaysIncludedShaders.GetArrayElementAtIndex(i);
                list.Add(element.objectReferenceValue);
            }
            list.Sort((lhs, rhs) => string.Compare(lhs.name, rhs.name));
            for (int i = 0; i < alwaysIncludedShaders.arraySize; i++)
            {
                var element = alwaysIncludedShaders.GetArrayElementAtIndex(i);
                element.objectReferenceValue = list[i];
            }
            graphicsManager.ApplyModifiedProperties();
        }
    }
}