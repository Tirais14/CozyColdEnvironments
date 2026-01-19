using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity
{
    public readonly struct SceneLoadingEvent
    {
        public readonly Scene scene { get; }
        public readonly LoadSceneMode loadSceneMode { get; }

        public SceneLoadingEvent(Scene scene, LoadSceneMode loadSceneMode)
        {
            this.scene = scene;
            this.loadSceneMode = loadSceneMode;
        }
    }
}
