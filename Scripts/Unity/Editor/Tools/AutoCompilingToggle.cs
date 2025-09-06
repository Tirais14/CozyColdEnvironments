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

        static AutoCompilingToggle()
        {
            CompilationPipeline.compilationFinished += AfterCompilation;
            if (PlayerPrefs.HasKey(AUTO_COMPILING)
                &&
                PlayerPrefs.GetInt(AUTO_COMPILING) == 0
                )
                DisableAutoCompiling(false);
        }

        //Cause issues
        [MenuItem("Editor Scripts/Compiling/Force Compile And Disable &r")]
        public static void ForceCompileAndDisableCompiling()
        {
            EditorApplication.UnlockReloadAssemblies();
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
            CompilationPipeline.RequestScriptCompilation();

            if (compilationFinishedSubscriptionCount > 0)
            {
                CompilationPipeline.compilationFinished += AfterCompilation;
                compilationFinishedSubscriptionCount++;
            }

            Debug.Log($"{nameof(AutoCompilingToggle)}: Editor force compiling initiated.");
        }

        [MenuItem("Editor/Compiling/Enable &e")]
        public static void EnableAutoCompiling()
        {
            EditorApplication.UnlockReloadAssemblies();
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
            CompilationPipeline.RequestScriptCompilation();
            PlayerPrefs.SetInt(AUTO_COMPILING, 1);
            PlayerPrefs.Save();

            Debug.Log($"{nameof(AutoCompilingToggle)}: Auto compiling enabled.");
        }

        [MenuItem("Editor/Compiling/Disable &d")]
        public static void DisableAutoCompiling()
        {
            DisableAutoCompiling(true);
        }
        private static void DisableAutoCompiling(bool log)
        {
            EditorApplication.LockReloadAssemblies();
            PlayerPrefs.SetInt(AUTO_COMPILING, 0);
            PlayerPrefs.Save();

            if (log)
                Debug.Log($"{nameof(AutoCompilingToggle)}: Auto compiling disabled.");
        }

        private static void AfterCompilation(object _)
        {
            DisableAutoCompiling();

            CompilationPipeline.compilationFinished -= AfterCompilation;
            compilationFinishedSubscriptionCount--;

            CCDebug.PrintLog($"{nameof(AutoCompilingToggle)}: Editor force compiling ended.", typeof(AutoCompilingToggle));
        }
    }
}
