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
        private readonly static ReactiveCommand<(Scene previous, Scene current)> activeSceneChangesCmd = new();
        private static ReactiveCommand<(Scene scene, LoadSceneMode mode)>? sceneLoadedCmd;
        private static ReactiveCommand<Scene>? sceneUnloadedCmd;

        public static Scene ActiveScene { get; private set; }
       
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

        public static Observable<(Scene froMScene, Scene toScene)> ObserveActiveSceneChanged()
        {
            return activeSceneChangesCmd;
        }

        private static void BindActiveSceneChangedEvent()
        {
            SceneManager.activeSceneChanged += (fromScene, toScene) =>
            {
                ActiveScene = toScene;
                activeSceneChangesCmd.Execute((fromScene, toScene));
            };
        }
    }
}
