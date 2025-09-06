using CCEnvs.Common;
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

            if (compilationFinishedSubscriptionCount > 0)
            {
                CompilationPipeline.compilationFinished += AfterCompilation;
                compilationFinishedSubscriptionCount++;
            }

            CCDebug.PrintLog($"{nameof(AutoCompilingToggle)}: Editor force compiling initiated.");
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
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
            CompilationPipeline.RequestScriptCompilation();

            if (!isInternal)
            {
                PlayerPrefs.SetInt(AUTO_COMPILING, 1);
                PlayerPrefs.Save();
                CCDebug.PrintLog($"{nameof(AutoCompilingToggle)}: Auto compiling enabled.");
            }
        }

        private static void DisableAutoCompiling(bool isInternal)
        {
            EditorApplication.LockReloadAssemblies();

            if (!isInternal)
            {
                PlayerPrefs.SetInt(AUTO_COMPILING, 0);
                PlayerPrefs.Save();
                CCDebug.PrintLog($"{nameof(AutoCompilingToggle)}: Auto compiling disabled.");
            }
        }

        private static void AfterCompilation(object _)
        {
            if (!IsCompilationEnabled)
                DisableAutoCompiling();

            CompilationPipeline.compilationFinished -= AfterCompilation;
            compilationFinishedSubscriptionCount--;

            CCDebug.PrintLog($"{nameof(AutoCompilingToggle)}: Editor force compiling ended.", typeof(AutoCompilingToggle));
        }
    }
}
