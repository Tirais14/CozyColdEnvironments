using CCEnvs.FuncLanguage;
using R3;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity
{
    public static class SceneManagerHelper
    {
        private readonly static ReactiveCommand<(Scene prevScene, Scene currentScene)> activeSceneChangesCmd = new();
        
        private static ReactiveCommand<(Scene scene, LoadSceneMode mode)>? sceneLoadedCmd;
        private static ReactiveCommand<Scene>? sceneUnloadedCmd;

        private readonly static ReactiveProperty<Scene> activeScene = new(SceneManager.GetActiveScene());

        public static Scene ActiveScene => activeScene.Value;

        static SceneManagerHelper()
        {
            BindActiveSceneChangedEvent();
        }

        public static Scene[] GetLoadedScenes()
        {
            int sceneCount = SceneManager.sceneCount;
            var scenes = new Scene[sceneCount];

            for (int i = 0; i < sceneCount; i++)
                scenes[i] = SceneManager.GetSceneAt(i);

            return scenes;
        }

        public static SceneInfo[] GetLoadedSceneInfos()
        {
            return GetLoadedScenes().Select(scene => scene.GetSceneInfo()).ToArray();
        }

        public static Maybe<Scene> FindScene(string name, bool ignoreCase = false)
        {
            foreach (var scene in GetLoadedScenes())
            {
                if (scene.name.EqualsOrdinal(name, ignoreCase))
                    return scene;
            }

            return Maybe<Scene>.None;
        }

        public static Observable<(Scene scene, LoadSceneMode mode)> ObserveSceneLoaded()
        {
            if (sceneLoadedCmd is null)
            {
                sceneLoadedCmd = new ReactiveCommand<(Scene scene, LoadSceneMode mode)>();

                SceneManager.sceneLoaded += (scene, mode) =>
                {
                    sceneLoadedCmd.Execute((scene, mode));
                };
            }

            return sceneLoadedCmd;
        }

        public static Observable<Scene> ObserveSceneUnloaded()
        {
            if (sceneUnloadedCmd is null)
            {
                sceneUnloadedCmd = new ReactiveCommand<Scene>();

                SceneManager.sceneUnloaded += scene =>
                {
                    sceneUnloadedCmd.Execute(scene);
                };
            }

            return sceneUnloadedCmd;
        }

        public static Observable<(Scene previous, Scene current)> ObserveActiveSceneChanged()
        {
            return activeSceneChangesCmd;
        }

        public static Observable<Scene> ObserveActiveScene()
        {
            return activeScene;
        }

        private static void BindActiveSceneChangedEvent()
        {
            SceneManager.activeSceneChanged += (fromScene, toScene) =>
            {
                activeScene.Value = toScene;
                activeSceneChangesCmd.Execute((fromScene, toScene));
            };
        }
    }
}
