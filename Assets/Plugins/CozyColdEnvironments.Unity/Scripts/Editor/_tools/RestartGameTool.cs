using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.UnityEditor;
using Cysharp.Threading.Tasks;
using SuperLinq;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CCEnvs.Unity.EditorC
{
    public static class RestartGameTool
    {
        public static bool IsRestarted { get; }

        [MenuItem(EditorHelper.TOOLS_TAB_NAME + "/" + EditorHelper.CC_TAB + "/Restart Scene")]
        public static void Execute()
        {
            if (!Application.isPlaying)
                return;

            var scenes = Enumerable.Range(0, SceneManager.sceneCount)
                .Select(i => SceneManager.GetSceneAt(i))
                .OrderBy(scene => scene.isSubScene.Resolve().If(() => 0).Else(() => 1).Raw);

            var sceneNames = scenes.Select(scene => scene.name).ToArray();

            foreach (var scene in scenes)
            {
                try
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
                catch (Exception ex)
                {
                    typeof(RestartGameTool).PrintException(ex);
                }
            }

            UniTask.Create(async () =>
            {

                await SceneManager.LoadSceneAsync(sceneNames.First(), LoadSceneMode.Single);

                foreach (var name in sceneNames.Slice(1, sceneNames.Length - 2))
                {
                    await SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
                }
            }).Forget();
        }
    }
}
