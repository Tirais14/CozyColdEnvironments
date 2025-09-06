using CCEnvs.Common;
using CCEnvs.FileSystem;
using CCEnvs.Json;
using System;
using UnityEngine;

#nullable enable
#pragma warning disable S3168
namespace CCEnvs.Unity.Json.EditorC
{
    public static class UJsonWordDictionary
    {
        private static bool inProcess;

        public static async void Create(string[]? includeNamespaces = null,
                                        string[]? excludeNamespaces = null,
                                        FSPath path = default)
        {
            try
            {
                if (inProcess)
                    return;

                CCDebug.PrintLog($"Creating {nameof(UJsonWordDictionary).TrimFirst()}.");

                inProcess = true;

                if (Equals(path, default(FSPath)))
                    path = Application.dataPath.ToFilePath() - "Assets" + "External" + "Json" + "dictionary_generated.txt";

                await JsonWordDictionary.Create(path,
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
