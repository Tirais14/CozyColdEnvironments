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
    }
}
