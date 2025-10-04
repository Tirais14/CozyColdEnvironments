using CCEnvs.Diagnostics;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Immutable;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity.Essentials
{
    public class GameEntryPoint : ASceneEntryPoint
    {
        [SerializeField]
        protected LevelLoadOrder? levelLoadOrder;

        protected override async void Start()
        {
            base.Start();

            Setup();
            await LoadEssentials();
        }

        /// <summary>
        /// Calls before loading scenes
        /// </summary>
        protected virtual void Setup()
        {
        }

        protected virtual void OnGameLoaded()
        {
        }

        private async UniTask LoadEssentials()
        {
            var loadInfos = levelLoadOrder == null
                            ?
                            ImmutableArray<LevelLoadInfo>.Empty
                            :
                            levelLoadOrder.Order.Value;

            foreach (var loadInfo in loadInfos)
            {
                try
                {
                    await LevelLoader.LoadLevelAsync(loadInfo.SceneKey.ToString(), loadInfo.LoadMode);

                    //Prevent unloading current scene
                    if (loadInfo.LoadMode == LoadSceneMode.Additive
                        &&
                        loadInfo.DestroyAfterActivated
                        )
                        await LevelLoader.UnloadLevelAsync(loadInfo.SceneKey.ToString());
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                    return;
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
