#if UNITY_EDITOR
using CCEnvs.Diagnostics;
using CCEnvs.UnityEditor;
using UnityEditor;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Editr
{
    [InitializeOnLoad]
    public static class AutoCompilingToggleTool
    {
        private const string AUTO_COMPILING = "AutoCompiling";

        private static bool IsCompilationEnabled => !PlayerPrefs.HasKey(AUTO_COMPILING)
                                                    ||
                                                    PlayerPrefs.GetInt(AUTO_COMPILING) == 1;

        static AutoCompilingToggleTool()
        {
            if (!IsCompilationEnabled)
                DisableAutoCompiling(isInternal: false);
            else
                EnableAutoCompiling(isInternal: false);
        }

        [MenuItem(EditorHelper.EDITOR_TAB_NAME + "/" + EditorHelper.CCENVS_TAB + "/" + EditorHelper.COMPILING_TAB_NAME + "/Enable &e")]
        public static void EnableAutoCompiling()
        {
            EnableAutoCompiling(isInternal: false);
        }

        [MenuItem(EditorHelper.EDITOR_TAB_NAME + "/" + EditorHelper.CCENVS_TAB + "/" + EditorHelper.COMPILING_TAB_NAME + "/Disable &d")]
        public static void DisableAutoCompiling()
        {
            DisableAutoCompiling(isInternal: false);
        }

        private static void SetEnabled(bool isInternal)
        {
            PlayerPrefs.SetInt(AUTO_COMPILING, 1);
            PlayerPrefs.Save();

            if (!isInternal)
                CCDebug.Instance.PrintLog($"Auto compiling enabled.",
                                 typeof(AutoCompilingToggleTool));
        }

        private static void SetDisabled(bool isInternal)
        {
            PlayerPrefs.SetInt(AUTO_COMPILING, 0);
            PlayerPrefs.Save();

            if (!isInternal)
                CCDebug.Instance.PrintLog($"Auto compiling disabled.",
                                 typeof(AutoCompilingToggleTool));
        }

        private static void EnableAutoCompiling(bool isInternal)
        {
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
#endif