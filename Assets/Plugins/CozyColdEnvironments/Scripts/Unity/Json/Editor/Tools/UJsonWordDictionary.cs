using CCEnvs.Diagnostics;
using CCEnvs.Json;
using UnityEngine;
using CCEnvs.Files;

#nullable enable
#pragma warning disable S3168
namespace CCEnvs.Unity.Json.EditorC
{
    public static class UJsonWordDictionary
    {
        private static bool inProcess;

        public static async void Create(string[]? includeNamespaces = null,
                                        string[]? excludeNamespaces = null,
                                        Files.Path path = default)
        {
            try
            {
                if (inProcess)
                    return;

                CCDebug.PrintLog($"Creating {nameof(UJsonWordDictionary).TrimFirst()}.");

                inProcess = true;

                if (Equals(path, default(Files.Path)))
                    path = Application.dataPath.ToFilePath() - "Assets" + "External" + "Json" + "dictionary_generated.txt";

                await JsonIDEWordDictionary.Create(path,
                                                includeNamespaces,
                                                excludeNamespaces);
                inProcess = false;
                CCDebug.PrintLog($"Creation {nameof(UJsonWordDictionary).TrimFirst()} finished.");
            }
            catch (System.Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }
    }
}
