using CCEnvs.Diagnostics;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity.Essentials
{
    public class GameEntryPoint : ASceneEntryPoint
    {
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
            for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                try
                {
                    await SceneLoader.LoadSceneAsync(i);
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
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
