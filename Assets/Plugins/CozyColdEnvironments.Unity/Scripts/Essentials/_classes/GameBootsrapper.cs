using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using R3;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity.Essentials
{
    public sealed class GameBootsrapper : CCBehaviourStatic<GameBootsrapper>
    {
        [SerializeField]
        [Tooltip("Stops loading scenes by specified key. This is scene also will be laoded.")]
        private int finalSceneIdx = -1;

        [SerializeField]
        private bool destroyGameObjectOnFinished = true;

        [SerializeField]
        private string loadingScenePath;

        private Scene loadingScene;

        private ReactiveCommand<Unit>? gameBootstrappedCmd;

        protected override async void Start()
        {
            base.Start();
            await LoadScenesAsync();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            gameBootstrappedCmd?.Dispose();
        }

        public static Observable<Unit> ObserveGameBootrapped()
        {
            self.gameBootstrappedCmd ??= new ReactiveCommand<Unit>();

            return self.gameBootstrappedCmd;
        }

        private async UniTask OnGameLoadedAsync()
        {
            await UniTask.NextFrame(
                timing: PlayerLoopTiming.Initialization,
                destroyCancellationToken)
            ;

            if (loadingScene.IsValid())
            {
                await UniTask.WaitForSeconds(
                    duration: 3f,
                    ignoreTimeScale: true,
                    delayTiming: PlayerLoopTiming.Update
                    );

                await SceneManager.UnloadSceneAsync(loadingScene);
            }

            gameBootstrappedCmd?.Execute(Unit.Default);

            if (destroyGameObjectOnFinished)
                Destroy(gameObject);
            else
                Destroy(this);
        }

        private async UniTask LoadScenesAsync()
        {
            Scene scene = default;

            if (loadingScenePath.IsNotNullOrWhiteSpace())
            {
                await SceneManager.LoadSceneAsync(loadingScenePath, LoadSceneMode.Single);

                loadingScene = SceneManager.GetSceneByName(loadingScenePath);

                SceneManager.SetActiveScene(loadingScene);
            }

            for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                await SceneManager.LoadSceneAsync(i, LoadSceneMode.Additive);

                if (scene.IsValid())
                    await SceneManager.UnloadSceneAsync(scene);

                scene = SceneManager.GetSceneByBuildIndex(i);

                if (loadingScene.IsValid())
                    SceneManager.SetActiveScene(loadingScene);

                if (finalSceneIdx > -1 && i == finalSceneIdx)
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(i));
                    break;
                }
            }

            try
            {
                await OnGameLoadedAsync();
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }
    }
}
