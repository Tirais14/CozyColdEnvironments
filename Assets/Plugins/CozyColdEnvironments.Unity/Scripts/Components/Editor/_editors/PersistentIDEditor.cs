#if UNITY_EDITOR
using CCEnvs.UnityX.EditorSerialization;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Components.Editr
{
    [CustomEditor(typeof(PersistentID))]
    public class PersistentIDEditor : Editor
    {
        private static readonly FieldInfo valueField = typeof(PersistentID).GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo innerValueField = typeof(SerializedNullable<int>).GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo hasValueField = typeof(SerializedNullable<int>).GetField("hasValue", BindingFlags.NonPublic | BindingFlags.Instance);

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (valueField != null && innerValueField != null && hasValueField != null)
            {
                object? nullableObj = valueField.GetValue(target);

                if (nullableObj != null)
                {
                    int currentValue = (int)innerValueField.GetValue(nullableObj);
                    bool currentHasValue = (bool)hasValueField.GetValue(nullableObj);

                    EditorGUI.BeginChangeCheck();
                    int newValue = EditorGUILayout.IntField("ID", currentValue);

                    if (EditorGUI.EndChangeCheck())
                    {
                        innerValueField.SetValue(nullableObj, newValue);
                        hasValueField.SetValue(nullableObj, true);
                        valueField.SetValue(target, nullableObj);
                        EditorUtility.SetDirty(target);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("ID", "Error: value field is null");
                }
            }
            else
            {
                EditorGUILayout.LabelField("ID", "Error: Could not find fields via reflection");
            }

            EditorGUILayout.Space();

            GUI.backgroundColor = new Color(0.8f, 1f, 0.8f);
            if (GUILayout.Button("Generate ID", GUILayout.Height(30)))
            {
                GenerateNewID();
            }
            GUI.backgroundColor = Color.white;

            serializedObject.ApplyModifiedProperties();
        }

        private void GenerateNewID()
        {
            HashSet<int> existingIds = new HashSet<int>();

            // Находим все компоненты в сцене (включая неактивные объекты)
            PersistentID[] allComponents = Resources.FindObjectsOfTypeAll<PersistentID>();

            foreach (var comp in allComponents)
            {
                if (comp != null && comp.gameObject != null && comp.gameObject.scene.isLoaded)
                {
                    existingIds.Add(comp.Value);
                }
            }

            // Генерируем новый ID, которого нет в сцене
            int newId;
            do
            {
                newId = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            }
            while (existingIds.Contains(newId));

            // Применяем новый ID через рефлексию
            if (valueField != null && innerValueField != null && hasValueField != null)
            {
                object? nullableObj = valueField.GetValue(target);

                if (nullableObj != null)
                {
                    innerValueField.SetValue(nullableObj, newId);
                    hasValueField.SetValue(nullableObj, true);
                    valueField.SetValue(target, nullableObj);
                    EditorUtility.SetDirty(target);
                }
            }
        }
    }
}
#endif