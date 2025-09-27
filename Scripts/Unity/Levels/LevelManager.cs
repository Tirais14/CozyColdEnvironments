using CCEnvs.Async;
using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.EditorSerialization;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
#pragma warning disable S1117
namespace CCEnvs.Unity.Levels
{
    public sealed class LevelManager : CCBehaviourStatic<LevelManager>
    {
        [SerializeField]
        private SerializedTimeSpan delayBeforeSceneLoaded = new(TimeSpan.FromSeconds(1));

        private TimeSpan delayTimer;

        public static HashSet<IAsyncTaskRegistry> TaskRegistries { get; } = new(new AnonymousEqualityComparer<IAsyncTaskRegistry>((x, y) => x.GetType() == y.GetType(), x => x.GetType().GetHashCode()));

        protected override void OnAwake()
        {
            base.OnAwake();

            TaskRegistries.Add(CC.NeccesaryTasks);
        }

        private void LateUpdate()
        {
            if (!IsReadyToLoadLevel())
            {
                delayTimer = TimeSpan.Zero;
                return;
            }

            delayTimer += TimeSpan.FromSeconds(Time.deltaTime);
        }
        /// <summary>Do not add this task to any of <see cref="LevelManager.TaskRegistries"/></summary>
        /// <param name="sceneKey">build scene id or scene name</param>
        /// <exception cref="ArgumentException"></exception>
        public static async UniTask<Scene> LoadLevelAsync(object sceneKey,
            LoadSceneParameters loadParams,
            Action<AsyncOperation>? beforeLoading = null)
        {
            CC.Validate.ArgumentNull(sceneKey, nameof(sceneKey));

            Self.enabled = true;
            Self.PrintLog($"Loading started. Scene: {sceneKey}.");

            await UniTask.WaitWhile(() => Self.delayTimer < Self.delayBeforeSceneLoaded);

            AsyncOperation op;
            if (sceneKey is string sceneName)
                op = SceneManager.LoadSceneAsync(sceneName, loadParams);
            else if (sceneKey is int sceneID)
                op = SceneManager.LoadSceneAsync(sceneID, loadParams);
            else
                throw new ArgumentException($"Invalid {nameof(sceneKey)}. Type: {sceneKey.GetType()}");

            beforeLoading?.Invoke(op);

            await op;

            return SceneManager.GetActiveScene();
        }
        /// <inheritdoc cref="LoadLevelAsync"/>
        public static async UniTask<Scene> LoadLevelAsync(object sceneKey,
            LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            return await LoadLevelAsync(sceneKey, new LoadSceneParameters(loadMode));
        }

        public static async UniTask UnloadLevelAsync(object sceneKey)
        {
            CC.Validate.ArgumentNull(sceneKey, nameof(sceneKey));

            AsyncOperation op;
            if (sceneKey is string sceneName)
                op = SceneManager.UnloadSceneAsync(sceneName);
            else if (sceneKey is int sceneID)
                op = SceneManager.UnloadSceneAsync(sceneID);
            else
                throw new ArgumentException($"Invalid {nameof(sceneKey)}. Type: {sceneKey.GetType()}");

            if (op != null)
                await op;
        }

        public static bool IsReadyToLoadLevel()
        {
            return TaskRegistries.IsEmpty() || !TaskRegistries.Any(x => x.HasTasks);
        }
    }
}
