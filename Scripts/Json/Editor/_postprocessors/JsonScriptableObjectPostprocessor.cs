using System.IO;
using UnityEditor;
using UnityEngine;
using UTIRLib.Json;

namespace UTIRLib.FileSystem.Json.UnityEditor
{
    public class JsonScriptableObjectPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets,
                                                   string[] deletedAssets,
                                                   string[] movedAssets,
                                                   string[] movedFromAssetPaths)
        {
            foreach (var assetPath in importedAssets)
            {
                if (Path.GetExtension(assetPath).Equals(".json", System.StringComparison.OrdinalIgnoreCase))
                {
                    var assetCandidate = Path.ChangeExtension(assetPath, ".asset");

                    // Если рядом есть ScriptableObject
                    var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetCandidate);
                    if (so != null && so.GetType().BaseType != null &&
                        so.GetType().BaseType.IsGenericType &&
                        so.GetType().BaseType.GetGenericTypeDefinition() == typeof(JsonScriptableObject<>))
                    {
                        so.GetType().GetMethod("ReloadFromJsonMenu")?.Invoke(so, null);
                        EditorUtility.SetDirty(so);
                        Debug.Log($"[JSON Sync] {assetPath} → {assetCandidate} обновлён");
                    }
                }
            }
        }
    }
}
