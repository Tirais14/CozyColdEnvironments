using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public sealed class SaveLoaderLazyObjectRestorer : IDisposable, IFrameRunnerWorkItem
    {
        private readonly C5.IntervalHeap<MonoBehaviourInfo> monoBehs = new();

        private readonly SaveObjectRestorer saveLoader;

        private long everyFrame = 0L;

        private int batchSize = 100;

        public long EveryFrame {
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

        public SaveLoaderLazyObjectRestorer(
            SaveObjectRestorer saveLoader,
            FrameProvider? frameProvider = null
            )
        {
            Guard.IsNotNull(saveLoader, nameof(saveLoader));

            this.saveLoader = saveLoader;
            (frameProvider ?? UnityFrameProvider.PostLateUpdate).Register(this);
        }

        public void Enqueue(
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

            var monoBehInfo = new MonoBehaviourInfo(
                monoBeh, 
                key,
                saveGroup,
                callbackState,
                callback
                );

            monoBehs.Add(monoBehInfo);
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            while (monoBehs.Count > 0)
                monoBehs.DeleteMin();

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

            MonoBehaviourInfo monoBeh;

            var toProcessCount = Mathf.Clamp(monoBehs.Count, 0, BatchSize);

            bool isMonoBehRestored;

            for (int i = 0; i < toProcessCount; i++)
            {
                monoBeh = monoBehs.DeleteMin();

                if (monoBeh.IsNull())
                {
                    toProcessCount = Mathf.Clamp(toProcessCount + 1, 0, monoBehs.Count);
                    continue;
                }

                if (!monoBeh.Value.didStart)
                {
                    monoBehs.Add(monoBeh);
                    return;
                }

                isMonoBehRestored =  saveLoader.TryRestoreObjectCore(
                    monoBeh.Value,
                    monoBeh.Key,
                    monoBeh.SaveGroup
                    );

                monoBeh.TryInvokeCallback(isMonoBehRestored);
            }
        }

        bool IFrameRunnerWorkItem.MoveNext(long frameCount)
        {
            if (disposed)
                return false;

            if (frameCount % EveryFrame != 0)
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
                return Key == other.Key &&
                       EqualityComparer<MonoBehaviour>.Default.Equals(Value, other.Value) &&
                       EqualityComparer<SaveGroup>.Default.Equals(SaveGroup, other.SaveGroup);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Key, Value, SaveGroup);
            }

            public int CompareTo(MonoBehaviourInfo other)
            {
                return -Comparer<bool>.Default.Compare(Value.didStart, other.Value.didStart);
            }
        }
    }
}
