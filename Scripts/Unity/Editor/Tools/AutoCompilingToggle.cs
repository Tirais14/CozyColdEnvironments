using CCEnvs.Diagnostics;
using UnityEditor;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Editor
{
    [InitializeOnLoad]
    public static class AutoCompilingToggle
    {
        private const string AUTO_COMPILING = "AutoCompiling";

        private static bool IsCompilationEnabled => !PlayerPrefs.HasKey(AUTO_COMPILING)
                                                    ||
                                                    PlayerPrefs.GetInt(AUTO_COMPILING) == 1;

        static AutoCompilingToggle()
        {
            if (!IsCompilationEnabled)
                DisableAutoCompiling(isInternal: true);
            else
                EnableAutoCompiling(isInternal: true);
        }

        [MenuItem("Editor/Compiling/Enable &e")]
        public static void EnableAutoCompiling()
        {
            if (IsCompilationEnabled)
                return;

            EditorApplication.UnlockReloadAssemblies();

            SetEnabled(isInternal: false);
        }

        [MenuItem("Editor/Compiling/Disable &d")]
        public static void DisableAutoCompiling()
        {
            DisableAutoCompiling(isInternal: false);
        }

        private static void SetEnabled(bool isInternal)
        {
            PlayerPrefs.SetInt(AUTO_COMPILING, 1);
            PlayerPrefs.Save();

            if (!isInternal)
                CCDebug.PrintLog($"{nameof(AutoCompilingToggle)}: Auto compiling enabled.",
                                 typeof(AutoCompilingToggle));
        }

        private static void SetDisabled(bool isInternal)
        {
            PlayerPrefs.SetInt(AUTO_COMPILING, 0);
            PlayerPrefs.Save();

            if (!isInternal)
                CCDebug.PrintLog($"{nameof(AutoCompilingToggle)}: Auto compiling disabled.",
                                 typeof(AutoCompilingToggle));
        }

        private static void EnableAutoCompiling(bool isInternal)
        {
            if (IsCompilationEnabled)
                return;

            EditorApplication.UnlockReloadAssemblies();

            SetEnabled(isInternal);
        }

        private static void DisableAutoCompiling(bool isInternal)
        {
            EditorApplication.LockReloadAssemblies();

            SetDisabled(isInternal);
        }
    }
}
