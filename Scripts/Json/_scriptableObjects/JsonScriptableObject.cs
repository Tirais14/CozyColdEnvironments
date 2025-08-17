using Newtonsoft.Json;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

#nullable enable
#pragma warning disable S2094
namespace UTIRLib.Json
{
    public abstract class JsonScriptableObject : ScriptableObject
    {
        
    }
    /// <typeparam name="T">Serializable data object</typeparam>
    public abstract class JsonScriptableObject<T>
        : 
        JsonScriptableObject

        where T : new()
    {
        [field: SerializeField]
        public T Data { get; protected set; } = default!;

        private string JsonPath => Path.ChangeExtension(AssetDatabase.GetAssetPath(this), ".json");

        private void OnEnable()
        {
#if UNITY_EDITOR
            LoadFromJson();
#endif
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            SaveToJson();
#endif
        }

        private void LoadFromJson()
        {
            if (!File.Exists(JsonPath)) 
                return;

            try
            {
                var json = File.ReadAllText(JsonPath);
                Data = JsonConvert.DeserializeObject<T>(json, JsonHelper.Converters);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load JSON: {e.Message}");
            }
        }

        private void SaveToJson()
        {
            try
            {
                var json = JsonConvert.SerializeObject(Data, Formatting.Indented, JsonHelper.Converters);
                File.WriteAllText(JsonPath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save JSON: {e.Message}");
            }
        }
    }
}
