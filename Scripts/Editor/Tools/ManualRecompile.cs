using UnityEditor;
using UnityEngine;

#nullable enable
namespace UTIRLib.Unity.Editor
{
    [InitializeOnLoad]
    public static class ManualRecompile
    {
        static ManualRecompile()
        {
            EditorApplication.LockReloadAssemblies();
        }

        [MenuItem("Tools/Manual Compile/Compile Now &r")]
        public static void ManualCompile()
        {
            Debug.Log("Manual compilation initiated...");

            // ¬ременно разрешаем всЄ
            EditorApplication.UnlockReloadAssemblies();

            // явно запускаем обновление ассетов и компил€цию
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();

            // ƒаем врем€ на начало процесса компил€ции
            EditorApplication.delayCall += () =>
            {
                // ∆дем окончани€ компил€ции перед повторной блокировкой
                WaitForCompilationToFinish(() =>
                {
                    EditorApplication.LockReloadAssemblies();
                    Debug.Log("Manual compilation finished. Auto-refresh locked again.");
                });
            };
        }

        private static void WaitForCompilationToFinish(System.Action onFinished)
        {
            if (!EditorApplication.isCompiling)
            {
                onFinished?.Invoke();
                return;
            }

            EditorApplication.update += WaitForCompilation;
            void WaitForCompilation()
            {
                if (!EditorApplication.isCompiling)
                {
                    EditorApplication.update -= WaitForCompilation;
                    onFinished?.Invoke();
                }
            }
        }
    }
}
