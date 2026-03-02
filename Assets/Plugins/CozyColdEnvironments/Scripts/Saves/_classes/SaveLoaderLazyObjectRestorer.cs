#if UNITY_2017_1_OR_NEWER
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Saves
{
    public sealed class SaveLoaderLazyObjectRestorer : IDisposable, IFrameRunnerWorkItem
    {
        private readonly ConcurrentQueue<MonoBehaviourInfo> monoBehs = new();

        private readonly HashSet<MonoBehaviour> enquedMonoBehs = new();

        private readonly SaveObjectRestorer saveLoader;

        private long everyFrame = 0L;

        private int batchSize = 100;

        public long ExecuteEveryFrame {
            get => everyFrame;
            set => everyFrame = Math.Clamp(value, 0L, long.MaxValue);
        }

        public int BatchSize {
            get => batchSize;
            set
            {
                if (value <= 0)
                    batchSize = int.MaxValue;
                else
                    batchSize = value;
            }
        }

        public object SyncRoot { get; } = new();

        public SaveLoaderLazyObjectRestorer(
            SaveObjectRestorer saveLoader,
            FrameProvider? frameProvider = null
            )
        {
            Guard.IsNotNull(saveLoader, nameof(saveLoader));

            this.saveLoader = saveLoader;
            (frameProvider ?? UnityFrameProvider.PostLateUpdate).Register(this);
        }

        public void TryEnqueue(
            MonoBehaviour monoBeh,
            string key,
            SaveGroup saveGroup,
            object? callbackState = null,
            Action<object?, bool>? callback = null
            )
        {
            ThrowIfDisposed();

            CC.Guard.IsNotNull(monoBeh, nameof(monoBeh));
            Guard.IsNotNull(key, nameof(key));
            Guard.IsNotNull(saveGroup, nameof(saveGroup));

            lock (SyncRoot)
            {
                if (enquedMonoBehs.Contains(monoBeh))
                {
                    if (CCDebug.Instance.IsEnabled)
                        this.PrintLog($"Mono Behaviour is already enqueued");

                    return;
                }
            }

            var monoBehInfo = new MonoBehaviourInfo(
                monoBeh,
                key,
                saveGroup,
                callbackState,
                callback
                );

            monoBehs.Enqueue(monoBehInfo);

            lock (SyncRoot)
                enquedMonoBehs.Add(monoBeh);
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            monoBehs.Clear();

            disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        private void OnFrame()
        {
            if (monoBehs.IsEmpty())
                return;

            var toProcessCount = Mathf.Clamp(monoBehs.Count, 0, BatchSize);

            bool isMonoBehRestored;

            int processedCount = 0;

            using var notReadyMonoBehs = ListPool<MonoBehaviourInfo>.Shared.Get();

#if CC_DEBUG_ENABLED
            var loopFuse = LoopFuse.Create();
#endif

            while (processedCount++ < toProcessCount
                   &&
                   monoBehs.TryDequeue(out var monoBeh)
#if CC_DEBUG_ENABLED
                   &&
                   loopFuse.MoveNext()
#endif
                   )
            {
                if (monoBeh.Value.IsNull())
                {
                    toProcessCount = Mathf.Clamp(toProcessCount + 1, 0, monoBehs.Count);
                    continue;
                }

                if (!monoBeh.Value.didStart)
                {
                    notReadyMonoBehs.Value.Add(monoBeh);
                    continue;
                }

                isMonoBehRestored = saveLoader.TryRestoreObjectCore(
                    monoBeh.Value,
                    monoBeh.Key,
                    monoBeh.SaveGroup
                    );

                monoBeh.TryInvokeCallback(isMonoBehRestored);

                lock (SyncRoot)
                    enquedMonoBehs.Remove(monoBeh.Value);
            }

            int notReadyCount = notReadyMonoBehs.Value.Count;

            for (int i = 0; i < notReadyCount; i++)
                monoBehs.Enqueue(notReadyMonoBehs.Value[i]);
        }

        bool IFrameRunnerWorkItem.MoveNext(long frameCount)
        {
            if (disposed)
                return false;

            if (frameCount % ExecuteEveryFrame != 0)
                return true;

            OnFrame();

            return true;
        }

        private readonly struct MonoBehaviourInfo : IEquatable<MonoBehaviourInfo>, IComparable<MonoBehaviourInfo>
        {
            public readonly MonoBehaviour Value;

            public readonly string Key;

            public readonly SaveGroup SaveGroup;

            public readonly object? CallbackState;

            public readonly Action<object?, bool>? Callback;

            public MonoBehaviourInfo(
                MonoBehaviour value,
                string key,
                SaveGroup saveGroup,
                object? callbackState = null,
                Action<object?, bool>? callback = null
                )
            {
                Value = value;
                Key = key;
                SaveGroup = saveGroup;
                CallbackState = callbackState;
                Callback = callback;
            }

            public static bool operator ==(MonoBehaviourInfo left, MonoBehaviourInfo right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(MonoBehaviourInfo left, MonoBehaviourInfo right)
            {
                return !(left == right);
            }

            public static bool operator <(MonoBehaviourInfo left, MonoBehaviourInfo right)
            {
                return left.CompareTo(right) < 0;
            }

            public static bool operator <=(MonoBehaviourInfo left, MonoBehaviourInfo right)
            {
                return left.CompareTo(right) <= 0;
            }

            public static bool operator >(MonoBehaviourInfo left, MonoBehaviourInfo right)
            {
                return left.CompareTo(right) > 0;
            }

            public static bool operator >=(MonoBehaviourInfo left, MonoBehaviourInfo right)
            {
                return left.CompareTo(right) >= 0;
            }

            public readonly void TryInvokeCallback(bool state)
            {
                try
                {
                    Callback?.Invoke(CallbackState, state);
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }
            }

            public override bool Equals(object? obj)
            {
                return obj is MonoBehaviourInfo info && Equals(info);
            }

            public bool Equals(MonoBehaviourInfo other)
            {
                return Key == other.Key
                       &&
                       EqualityComparer<MonoBehaviour>.Default.Equals(Value, other.Value)
                       &&
                       EqualityComparer<SaveGroup>.Default.Equals(SaveGroup, other.SaveGroup)
                       &&
                       EqualityComparer<object?>.Default.Equals(CallbackState, other.CallbackState)
                       &&
                       EqualityComparer<object?>.Default.Equals(Callback, other.Callback);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Key, Value, SaveGroup, CallbackState, Callback);
            }

            public int CompareTo(MonoBehaviourInfo other)
            {
                return -Comparer<bool>.Default.Compare(Value.didStart, other.Value.didStart);
            }
        }
    }
}
#endif
