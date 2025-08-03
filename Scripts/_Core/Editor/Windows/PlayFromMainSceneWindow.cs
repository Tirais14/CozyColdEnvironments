using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable S2325
namespace UTIRLib.Unity.Editor
{
    public class PlayFromMainSceneWindow : EditorWindow
    {
        private static bool isEnabled;
        private static string mainScenePath;

        [MenuItem("Window/Play From Main Scene Settings")]
        public static void ShowWindow()
        {
            GetWindow<PlayFromMainSceneWindow>("Play Settings");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Play From Main Scene Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            isEnabled = EditorGUILayout.Toggle("Enable Feature", isEnabled);
            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(!isEnabled);
            EditorGUILayout.LabelField("Main Scene Path:");
            EditorGUILayout.BeginHorizontal();
            mainScenePath = EditorGUILayout.TextField(mainScenePath);
            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                string path = EditorUtility.OpenFilePanel("Select Scene", "Assets", "unity");
                if (!string.IsNullOrEmpty(path))
                {
                    mainScenePath = "Assets" + path.Substring(Application.dataPath.Length);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            if (GUI.changed)
            {
                SaveSettings();
            }
        }

        private void OnEnable()
        {
            LoadSettings();
        }

        private static void SaveSettings()
        {
            EditorPrefs.SetBool("PlayFromMainScene_Enabled", isEnabled);
            EditorPrefs.SetString("PlayFromMainScene_Path", mainScenePath);
        }

        private static void LoadSettings()
        {
            isEnabled = EditorPrefs.GetBool("PlayFromMainScene_Enabled", false);
            mainScenePath = EditorPrefs.GetString("PlayFromMainScene_Path", "");
        }

        [InitializeOnLoad]
        public static class PlayFromMainSceneHandler
        {
            private static List<string> originalScenes = new List<string>();
            private static bool isSwitching = false;
            private static Object[] originalSelection; // Сохраняем текущее выделение

            static PlayFromMainSceneHandler()
            {
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            }

            private static void OnPlayModeStateChanged(PlayModeStateChange state)
            {
                if (!isEnabled || string.IsNullOrEmpty(mainScenePath))
                    return;

                if (state == PlayModeStateChange.ExitingEditMode && !isSwitching)
                {
                    if (SceneManager.GetActiveScene().path == mainScenePath)
                        return;

                    if (!File.Exists(mainScenePath))
                    {
                        Debug.LogError($"Main scene not found: {mainScenePath}");
                        EditorApplication.isPlaying = false;
                        return;
                    }

                    isSwitching = true;

                    // Сохраняем текущее выделение
                    originalSelection = Selection.objects;

                    // Очищаем выделение перед переключением
                    Selection.activeObject = null;

                    try
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            SaveOriginalScenes();
                            EditorSceneManager.OpenScene(mainScenePath, OpenSceneMode.Single);
                        }
                        else
                        {
                            EditorApplication.isPlaying = false;
                        }
                    }
                    finally
                    {
                        isSwitching = false;

                        // Восстанавливаем выделение после операции
                        if (!EditorApplication.isPlaying)
                        {
                            Selection.objects = originalSelection;
                        }
                    }
                }
                // Восстанавливаем оригинальные сцены при возврате в редактор
                else if (state == PlayModeStateChange.EnteredEditMode)
                {
                    if (originalScenes.Count > 0 && !isSwitching)
                    {
                        isSwitching = true;
                        try
                        {
                            if (SceneManager.GetActiveScene().path != originalScenes[0])
                            {
                                EditorSceneManager.OpenScene(originalScenes[0], OpenSceneMode.Single);
                                for (int i = 1; i < originalScenes.Count; i++)
                                {
                                    EditorSceneManager.OpenScene(originalScenes[i], OpenSceneMode.Additive);
                                }
                            }
                        }
                        finally
                        {
                            originalScenes.Clear();
                            isSwitching = false;
                        }
                    }
                }
            }

            private static void SaveOriginalScenes()
            {
                originalScenes.Clear();
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    if (scene.IsValid() && !string.IsNullOrEmpty(scene.path))
                    {
                        originalScenes.Add(scene.path);
                    }
                }
            }
        }
    }
}
