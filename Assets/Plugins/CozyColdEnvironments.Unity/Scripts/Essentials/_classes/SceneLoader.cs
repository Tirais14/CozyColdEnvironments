using CCEnvs.Threading.Tasks;
using CCEnvs.Collections;
using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#nullable enable
#pragma warning disable S1117
namespace CCEnvs.Unity.Essentials
{
    ///// <summary>
    ///// Have similar methods as <see cref="SceneManager"/>, but awaits tasks in <see cref="TaskRegistries"/> before the new scene loaded and unloaded
    ///// </summary>
    //public sealed class SceneLoader : CCBehaviourStatic<SceneLoader>
    //{
    //    private static readonly Stopwatch stopwatch = new();

    //    [SerializeField]
    //    [Tooltip("In seconds")]
    //    private float delayBeforeSceneLoaded;

    //    private float delayTimer;

    //    public static event UnityAction? OnProcessStarted;
    //    public static event UnityAction? OnProcessFinished;

    //    /// <summary>
    //    /// By default contains <see cref="CC.NeccesaryTasks"/>
    //    /// </summary>
    //    public static HashSet<IAsyncTaskRegistry> TaskRegistries { get; } = new(new AnonymousEqualityComparer<IAsyncTaskRegistry>((x, y) => x.GetType() == y.GetType(), x => x.GetType().GetHashCode()));
    //    public static bool InProcess { get; private set; }
    //    public static bool IsReady => TaskRegistries.IsEmpty() || !TaskRegistries.Any(x => x.HasTasks);

    //    protected override void Awake()
    //    {
    //        base.Awake();

    //        TaskRegistries.Add(CC.NeccesaryTasks);
    //    }

    //    private void Update()
    //    {
    //        if (!InProcess)
    //            enabled = false;
    //    }

    //    private void LateUpdate()
    //    {
    //        if (!IsReady)
    //        {
    //            delayTimer = default;
    //            return;
    //        }

    //        delayTimer += Time.deltaTime;
    //    }
    //    /// <summary>Do not add this task to any of <see cref="SceneLoader.TaskRegistries"/></summary>
    //    /// <param name="sceneKey">build scene id or scene name</param>
    //    /// <exception cref="ArgumentException"></exception>
    //    public static async UniTask<Scene> LoadSceneAsync(object sceneKey,
    //        LoadSceneParameters loadParams,
    //        Action<AsyncOperation>? beforeLoading = null)
    //    {
    //        CC.Guard.IsNotNull(sceneKey, nameof(sceneKey));

    //        if (IsBusy())
    //            return default;

    //        StartProcessing($"Loading level: {sceneKey}");

    //        try
    //        {
    //            await Delay();

    //            AsyncOperation op;
    //            if (sceneKey is string sceneName)
    //                op = SceneManager.LoadSceneAsync(sceneName, loadParams);
    //            else if (sceneKey is int sceneID)
    //                op = SceneManager.LoadSceneAsync(sceneID, loadParams);
    //            else
    //                throw new ArgumentException($"Invalid {nameof(sceneKey)}. Type: {sceneKey.GetType()}");

    //            beforeLoading?.Invoke(op);
    //            await op;

    //            return SceneManager.GetActiveScene();
    //        }
    //        finally
    //        {
    //            FinishProcessing($"Loading level: {sceneKey}");
    //        }
    //    }
    //    /// <inheritdoc cref="LoadSceneAsync"/>
    //    public static async UniTask<Scene> LoadSceneAsync(object sceneKey,
    //        LoadSceneMode loadMode = LoadSceneMode.Single)
    //    {
    //        return await LoadSceneAsync(sceneKey, new LoadSceneParameters(loadMode));
    //    }

    //    public static async UniTask UnloadLevelAsync(object sceneKey)
    //    {
    //        CC.Guard.IsNotNull(sceneKey, nameof(sceneKey));

    //        if (IsBusy())
    //            return;

    //        StartProcessing($"Unloading level: {sceneKey}");

    //        try
    //        {
    //            await Delay();

    //            AsyncOperation op;
    //            if (sceneKey is string sceneName)
    //                op = SceneManager.UnloadSceneAsync(sceneName);
    //            else if (sceneKey is int sceneID)
    //                op = SceneManager.UnloadSceneAsync(sceneID);
    //            else
    //                throw new ArgumentException($"Invalid {nameof(sceneKey)}. Type: {sceneKey.GetType()}");

    //            if (op != null)
    //                await op;
    //        }
    //        finally
    //        {
    //            FinishProcessing($"Unloading level: {sceneKey}");
    //        }
    //    }

    //    private static async UniTask Delay()
    //    {
    //        await UniTask.WaitWhile(() => self.delayTimer < self.delayBeforeSceneLoaded);
    //    }

    //    private static bool IsBusy()
    //    {
    //        if (InProcess)
    //        {
    //            self.PrintError("Previous operation is not finished.");
    //            return true;
    //        }

    //        return false;
    //    }

    //    private static void StartProcessing(string opName)
    //    {
    //        stopwatch.Start();
    //        InProcess = true;
    //        self.enabled = true;
    //        self.PrintLog($"{opName} started.");

    //        try
    //        {
    //            OnProcessStarted?.Invoke();
    //        }
    //        catch (Exception ex)
    //        {
    //            self.PrintException(ex);
    //        }
    //    }

    //    private static void FinishProcessing(string opName)
    //    {
    //        stopwatch.Stop();
    //        InProcess = false;
    //        self.enabled = false;
    //        self.PrintLog($"{opName} finished. Operation duration: {stopwatch.ElapsedMilliseconds} ms.");
    //        stopwatch.Reset();

    //        try
    //        {
    //            OnProcessFinished?.Invoke();
    //        }
    //        catch (Exception ex)
    //        {
    //            self.PrintException(ex);
    //        }
    //    }
    //}
}
