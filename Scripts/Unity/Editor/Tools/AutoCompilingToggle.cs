using CCEnvs.Async;
using CCEnvs.Common;
using CCEnvs.Diagnostics;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Editor
{
    [InitializeOnLoad]
    public static class AutoCompilingToggle
    {
        private const string AUTO_COMPILING = "AutoCompiling";
        private static int compilationFinishedSubscriptionCount;
        private static bool compilationInProgress;

        private static bool IsCompilationEnabled => !PlayerPrefs.HasKey(AUTO_COMPILING)
                                                    ||
                                                    PlayerPrefs.GetInt(AUTO_COMPILING) == 1;

        static AutoCompilingToggle()
        {
            if (!IsCompilationEnabled)
                DisableAutoCompiling(isInternal: true);
        }

        //Cause issues
        [MenuItem("Editor/Compiling/Force Compile &r")]
        public static void ForceCompile()
        {
            EnableAutoCompiling(isInternal: true);

            if (compilationFinishedSubscriptionCount < 1)
            {
                _ = AfterCompilation();
                CompilationPipeline.compilationStarted += OnCompilationStarted;
                compilationFinishedSubscriptionCount++;
            }

            CCDebug.PrintLog($"{nameof(AutoCompilingToggle)}: Editor force compiling initiated.", typeof(AutoCompilingToggle));
        }

        [MenuItem("Editor/Compiling/Enable &e")]
        public static void EnableAutoCompiling()
        {
            EnableAutoCompiling(isInternal: false);
        }

        [MenuItem("Editor/Compiling/Disable &d")]
        public static void DisableAutoCompiling()
        {
            DisableAutoCompiling(isInternal: false);
        }

        private static void EnableAutoCompiling(bool isInternal)
        {
            EditorApplication.UnlockReloadAssemblies();
            _ = DelayedAssetDatabaseRefresh();
            //EditorUtility.RequestScriptReload();
            //CompilationPipeline.RequestScriptCompilation();

            if (!isInternal)
            {
                PlayerPrefs.SetInt(AUTO_COMPILING, 1);
                PlayerPrefs.Save();
                CCDebug.PrintLog($"{nameof(AutoCompilingToggle)}: Auto compiling enabled.", typeof(AutoCompilingToggle));
            }
        }

        private static void DisableAutoCompiling(bool isInternal)
        {
            EditorApplication.LockReloadAssemblies();

            if (!isInternal)
            {
                PlayerPrefs.SetInt(AUTO_COMPILING, 0);
                PlayerPrefs.Save();
                CCDebug.PrintLog($"{nameof(AutoCompilingToggle)}: Auto compiling disabled.", typeof(AutoCompilingToggle));
            }
        }

        private static async Task AfterCompilation()
        {
            if (!IsCompilationEnabled)
                DisableAutoCompiling();

            await Task.Delay(2000);

            await TaskHelper.WaitWhile(() => compilationInProgress
                                       ||
                                       EditorApplication.isCompiling);

            compilationInProgress = false;
            CompilationPipeline.compilationStarted -= OnCompilationStarted;
            compilationFinishedSubscriptionCount--;

            CCDebug.PrintLog($"{nameof(AutoCompilingToggle)}: Editor force compiling ended.", typeof(AutoCompilingToggle));
        }

        private static async Task DelayedAssetDatabaseRefresh()
        {
            await Task.Delay(1000);

            AssetDatabase.Refresh();
            CCDebug.PrintLog($"{nameof(AutoCompilingToggle)}: Asset database refreshed", typeof(AutoCompilingToggle));
            EditorUtility.RequestScriptReload();
            CompilationPipeline.RequestScriptCompilation();
            CCDebug.PrintLog($"{nameof(AutoCompilingToggle)}: Compilation requested", typeof(AutoCompilingToggle));
        }

        private static void OnCompilationStarted(object _)
        {
            compilationInProgress = true;
        }
    }
}
