using R3;
using System.Linq;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity.Components.Specialized
{
    public sealed class SceneActiveDependent : CCBehaviour
    {
        public bool destroyGameObjectOnExcluded;
        public bool excludeScenes;

        public SceneInfo[] activeScenes;

        protected override void Start()
        {
            base.Start();

            SceneManagerHelper.ObserveActiveSceneChanged()
                .Subscribe(this,
                static (ev, @this) =>
                {
                    @this.OnActiveSceneChanged(ev.current);
                })
                .AddDisposableTo(this);
        }

        private void OnActiveSceneChanged(Scene currentScene)
        {
            if (IsTargetScene(currentScene))
            {
                gameObject.SetActive(true);
                return;
            }

            if (destroyGameObjectOnExcluded)
            {
                Destroy(gameObject);
                return;
            }

            gameObject.SetActive(false);
        }

        private bool IsTargetScene(Scene scene)
        {
            return ScenePredciate(activeScenes.Contains(scene.GetSceneInfo()));
        }

        private bool ScenePredciate(bool state)
        {
            if (excludeScenes)
                return state;

            return !state;
        }
    }
}
