using CCEnvs.Diagnostics;
using CCEnvs.Files;
using CCEnvs.UnityEditor;
using SuperLinq;
using System.IO;
using UnityEditor;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.EditorC
{
    public static class VisualStudioClearCacheTool
    {
        [MenuItem(EditorHelper.EDITOR_TAB_NAME + "/" + EditorHelper.MAIN_TAB_NAME + "/" + EditorHelper.IDE_TAB_NAME + "/Visual Studio/Clear Cache")]
        public static void Execute()
        {
            try
            {
                var rootPath = new PathEntry(Application.dataPath) - "Assets" + ".vs";

                CCDebug.Instance.PrintLog($"Deleting {rootPath}.", new DebugContext(typeof(VisualStudioClearCacheTool)).Additive().Editor());
                var directory = new DirectoryInfo(rootPath);

                if (directory.Exists)
                {
                    FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
                    files.ForEach(x => x.Delete());
                }
            }
            catch (System.Exception ex)
            {
                CCDebug.Instance.PrintException(ex);
            }
        }
    }
}
