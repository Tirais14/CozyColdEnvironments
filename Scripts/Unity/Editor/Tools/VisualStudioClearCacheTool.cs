using CCEnvs.Diagnostics;
using CCEnvs.Files;
using SuperLinq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.EditorC
{
    public static class VisualStudioClearCacheTool
    {
        [MenuItem("Editor/IDE/Visual Studio/Clear Cache")]
        public static void Execute()
        {
            try
            {
                Files.Path rootPath = Application.dataPath.ToFilePath() - "Assets" + ".vs";

                CCDebug.PrintLog($"Deleting {rootPath}.", new DebugContext(typeof(VisualStudioClearCacheTool)).Additive().Editor());
                var directory = new DirectoryInfo(rootPath);

                if (directory.Exists)
                {
                    FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
                    files.ForEach(x => x.Delete());
                }
            }
            catch (System.Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }
    }
}
