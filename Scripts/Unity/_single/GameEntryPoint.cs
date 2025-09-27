using CCEnvs.Unity.Levels;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity
{
    public class GameEntryPoint : ASceneEntryPoint
    {
        [SerializeField, Min(0)]
        protected int gameLoadingStepCount;

        private readonly List<string> gameLoadingSceneNames = new();

        protected override void OnAwake()
        {
            base.OnAwake();

            for (int i = 0; i < gameLoadingStepCount; i++)
                gameLoadingSceneNames.Add(SceneManager.GetSceneByBuildIndex(i).name);
        }

        protected override async void OnStart()
        {
            base.OnStart();

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
            for (int i = 0; i < gameLoadingSceneNames.Count; i++)
            {
                await LevelLoader.LoadLevelAsync(gameLoadingSceneNames[i], LoadSceneMode.Single);

                //Prevent unloading current scene
                if (i != gameLoadingSceneNames.Count - 1)
                    await LevelLoader.UnloadLevelAsync(gameLoadingSceneNames[i]);
            }

            OnGameLoaded();
        }
    }
}
