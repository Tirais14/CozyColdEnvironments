using CCEnvs.FuncLanguage;
using System.Linq;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity
{
    public static class SceneManagerHelper
    {
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
    }
}
