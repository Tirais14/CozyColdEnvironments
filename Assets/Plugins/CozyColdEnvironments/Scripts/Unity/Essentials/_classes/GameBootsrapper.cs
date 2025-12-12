using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity.Essentials
{
    public class GameBootsrapper : CCBehaviourStatic<GameBootsrapper>
    {
        [SerializeField]
        [Tooltip("Stops loading scenes by specified key. This is scene also will be laoded.")]
        protected string finalSceneKey = string.Empty;

        [SerializeField]
        protected bool destroyGameObjectOnFinished = true;

        protected override async void Start()
        {
            base.Start();
            await LoadScenes();
        }

        protected virtual void OnGameLoaded()
        {
            UniTask.Create(this,
                static async @this =>
                {
                    await UniTask.NextFrame();
                    await UniTask.WaitForEndOfFrame();

                    if (@this.destroyGameObjectOnFinished)
                        Destroy(@this.gameObject);
                    else
                        Destroy(@this);
                })
                .Forget();
        }

        private async UniTask LoadScenes()
        {
            Scene scene;
            for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                scene = await SceneLoader.LoadSceneAsync(i);

                if (finalSceneKey.IsNotNullOrEmpty() && scene.IsValid())
                {
                    if (int.TryParse(finalSceneKey, out int buildIndex)
                        &&
                        scene.buildIndex == buildIndex)
                    {
                        break;
                    }
                    else if (scene.name == finalSceneKey 
                             || 
                             scene.path == finalSceneKey)
                    {
                        break;
                    }
                }
            }

            try
            {
                OnGameLoaded();
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }
    }
}
