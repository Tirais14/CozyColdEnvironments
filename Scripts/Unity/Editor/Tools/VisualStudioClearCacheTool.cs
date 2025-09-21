using CCEnvs.Diagnostics;
using CCEnvs.Files;
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
                Path rootPath = Application.dataPath.ToFilePath() - "Assets" + ".vs";

                CCDebug.PrintLog($"Deleting {rootPath}.", new DebugContext(typeof(VisualStudioClearCacheTool)).Additive().Editor());
                var directory = new DirectoryEntry(rootPath);

                if (directory.Exists)
                    directory.Delete();
            }
            catch (System.Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }
    }
}
