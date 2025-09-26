using CCEnvs.Async;
using CCEnvs.Diagnostics;
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
    public sealed class SceneLoader : CCBehaviourStatic<SceneLoader>
    {
        [SerializeField]
        private SerializedTimeSpan delayBeforeSceneLoaded = new(TimeSpan.FromSeconds(1));

        private TimeSpan delayTimer;

        public static List<IAsyncTaskRegistry> TaskRegistries { get; } = new();

        protected override void OnAwake()
        {
            base.OnAwake();

            TaskRegistries.Add(CC.NeccesaryTasks);
        }

        private void LateUpdate()
        {
            if (!IsReadyToLoadScene())
            {
                delayTimer = TimeSpan.Zero;
                return;
            }

            delayTimer += TimeSpan.FromSeconds(Time.deltaTime);
        }

        /// <param name="sceneKey">build scene id or scene name</param>
        /// <exception cref="ArgumentException"></exception>
        public static async UniTask LoadSceneAsync(object sceneKey,
            LoadSceneParameters loadParams)
        {
            CC.Validate.ArgumentNull(sceneKey, nameof(sceneKey));

            Self.enabled = true;
            Self.PrintLog($"Loading started. Scene: {sceneKey}.");

            await UniTask.WaitWhile(() => Self.delayTimer < Self.delayBeforeSceneLoaded);

            if (sceneKey is string sceneName)
                await SceneManager.LoadSceneAsync(sceneName, loadParams);
            else if (sceneKey is int sceneID)
                await SceneManager.LoadSceneAsync(sceneID, loadParams);
            else
                throw new ArgumentException($"Invalid {nameof(sceneKey)}. Type: {sceneKey.GetType()}");
        }
        /// <inheritdoc cref="LoadSceneAsync"/>
        public static async UniTask LoadSceneAsync(object sceneKey,
            LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            await LoadSceneAsync(sceneKey, new LoadSceneParameters(loadMode));
        }

        public static bool IsReadyToLoadScene()
        {
            return TaskRegistries.IsEmpty() || !TaskRegistries.Any(x => x.HasTasks);
        }
    }
}
