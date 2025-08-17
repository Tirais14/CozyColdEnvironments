using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UTIRLib.Json;
using UTIRLib.Reflection;

namespace UTIRLib.FileSystem.Json.UnityEditor
{
    [InitializeOnLoad]
    public static class JsonScriptableObjectBootstrap
    {
        static JsonScriptableObjectBootstrap()
        {
            EditorApplication.delayCall += SyncAllJsonScriptableObjects;
        }

        private static void SyncAllJsonScriptableObjects()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(JsonScriptableObject)}");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<JsonScriptableObject>(path);

                MethodInfo loadFromJsonMethod = asset.GetType()
                    .ForceGetMethods(BindingFlagsDefault.InstanceAll)
                    .First(x => x.Name == "LoadFromJson");

                loadFromJsonMethod.Invoke(asset, null);
            }
        }
    }
}
