using System.IO;
using UnityEditor;
using UnityEngine;
using UTIRLib.Json;

namespace UTIRLib.FileSystem.Json.UnityEditor
{
    [CustomEditor(typeof(JsonScriptableObject<>), true)]
    public class JsonScriptableObjectEditor : Editor
    {
        private JsonScriptableObject<object> _targetSO;

        private void OnEnable()
        {
            _targetSO = target as JsonScriptableObject<object>;

            // Подписываемся на изменения ассетов
            EditorApplication.projectChanged += OnProjectChanged;
        }

        private void OnDisable()
        {
            EditorApplication.projectChanged -= OnProjectChanged;
        }

        private void OnProjectChanged()
        {
            if (_targetSO == null) return;

            // Если JSON-файл изменился — подгружаем данные
            var jsonPath = Path.ChangeExtension(AssetDatabase.GetAssetPath(_targetSO), ".json");
            if (File.Exists(jsonPath))
            {
                _targetSO.GetType()
                         .GetMethod("ReloadFromJsonMenu")
                         ?.Invoke(_targetSO, null);

                // Обновим инспектор
                EditorUtility.SetDirty(_targetSO);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);
            var so = target;

            if (GUILayout.Button("⟳ Reload from JSON"))
            {
                so.GetType().GetMethod("ReloadFromJsonMenu")?.Invoke(so, null);
                EditorUtility.SetDirty(so);
            }

            if (GUILayout.Button("💾 Save to JSON"))
            {
                so.GetType().GetMethod("SaveToJsonMenu")?.Invoke(so, null);
            }
        }
    }
}
