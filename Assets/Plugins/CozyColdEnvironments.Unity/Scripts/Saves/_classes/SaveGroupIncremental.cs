using CCEnvs.Collections;
using CCEnvs.Pools;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public sealed class SaveGroupIncremental : SaveGroup
    {
        private readonly HashSet<IncrementallyObject> dirtyObjects = new();
        private readonly Dictionary<string, object> nonIncrementallyObjects = new();
        private readonly Dictionary<ISaveObjectIncrementally, OnSaveObjectIsDirtyChanged> incrementallyObjectBindings = new();

        private readonly IDisposable observableObjectAddBinding;
        private readonly IDisposable observableObjectRemoveBinding;

        public SaveGroupIncremental(
            SaveCatalog catalog,
            string? name = null
            )
            : 
            base(catalog, name)
        {
            observableObjectAddBinding = BindObservableObjectAdd();
            observableObjectRemoveBinding = BindObservableObjectRemove();
        }

        protected override PooledObject<List<SaveEntry>> CreateAndProcessSaveEntriesPooled()
        {
            var entryCount = dirtyObjects.Count + nonIncrementallyObjects.Count;

            var saveEntries = ListPool<SaveEntry>.Shared.Get();

            saveEntries.Value.TryIncreaseCapacity(entryCount);

            lock (SyncRoot)
            {
                foreach (var (key, obj) in dirtyObjects)
                {
                    if (!TryCreateAndProcessSaveEntry(key, obj, out var saveEntry))
                        continue;

                    saveEntries.Value.Add(saveEntry);
                }

                foreach (var (key, obj) in nonIncrementallyObjects)
                {
                    if (!TryCreateAndProcessSaveEntry(key, obj, out var saveEntry))
                        continue;

                    saveEntries.Value.Add(saveEntry);
                }
            }

            return saveEntries;
        }

        private bool disposed;
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                observableObjectAddBinding?.Dispose();
                observableObjectRemoveBinding?.Dispose();

                foreach (var obj in observableObjects)
                {
                    if (obj.Value is ISaveObjectIncrementally saveObjectIncrementally)
                        UnbindOnSaveObjectIsDirtyChanged(saveObjectIncrementally);
                }

                dirtyObjects.Clear();
                nonIncrementallyObjects.Clear();
            }

            disposed = true;

            base.Dispose(disposing);
        }

        //private void ThrowIfDisposed()
        //{
        //    if (!disposed)
        //        return;

        //    throw new ObjectDisposedException(GetType().Name);
        //}

        private void OnSaveObjectIsDirtyChangedCore(
            IncrementallyObject obj,
            bool state
            )
        {
            if (!state)
            {
                dirtyObjects.Remove(obj);
                return;
            }

            dirtyObjects.Add(obj);
        }

        private void BindOnSaveObjectIsDirtyChanged(string key, ISaveObjectIncrementally obj)
        {
            obj.OnSaveObjectIsDirtyChanged +=(obj, state) =>
            {
                var incObj = new IncrementallyObject(key, obj);

                OnSaveObjectIsDirtyChangedCore(incObj, state);
            };
        }

        private void UnbindOnSaveObjectIsDirtyChanged(ISaveObjectIncrementally obj)
        {
            if (!incrementallyObjectBindings.TryGetValue(obj, out var binding))
                return;

            obj.OnSaveObjectIsDirtyChanged -= binding;
        }

        private void OnObservableObjectAdd(DictionaryAddEvent<string, object> addEv)
        {
            var obj = addEv.Value;
            var key = addEv.Key;

            if (obj is ISaveObjectIncrementally saveObjectIncrementally)
                BindOnSaveObjectIsDirtyChanged(key, saveObjectIncrementally);
            else
                nonIncrementallyObjects.Add(key, obj);
        }

        private IDisposable BindObservableObjectAdd()
        {
            return observableObjects.ObserveDictionaryAdd(DisposeCancellationToken)
                .Subscribe(OnObservableObjectAdd);
        }

        private void OnObservableObjectRemove(DictionaryRemoveEvent<string, object> removeEv)
        {
            var obj = removeEv.Value;

            if (obj is ISaveObjectIncrementally saveObjectIncrementally)
                UnbindOnSaveObjectIsDirtyChanged(saveObjectIncrementally);
            else
                nonIncrementallyObjects.Remove(removeEv.Key);
        }

        private IDisposable BindObservableObjectRemove()
        {
            return observableObjects.ObserveDictionaryRemove(DisposeCancellationToken)
                .Subscribe(OnObservableObjectRemove);
        }

        private struct IncrementallyObject : IEquatable<IncrementallyObject>
        {
            private int? hash;

            public readonly string Key;

            public readonly ISaveObjectIncrementally Object;

            public IncrementallyObject(
                string key,
                ISaveObjectIncrementally obj
                )
            {
                hash = default;

                Key = key;
                Object = obj;
            }

            public readonly void Deconstruct(out string key, out ISaveObjectIncrementally obj)
            {
                key = Key;
                obj = Object;
            }

            public static bool operator ==(IncrementallyObject left, IncrementallyObject right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(IncrementallyObject left, IncrementallyObject right)
            {
                return !(left == right);
            }

            public readonly override bool Equals(object? obj)
            {
                return obj is IncrementallyObject info && Equals(info);
            }

            public readonly bool Equals(IncrementallyObject other)
            {
                return Key == other.Key
                       &&
                       EqualityComparer<ISaveObjectIncrementally>.Default.Equals(Object, other.Object);
            }

            public override int GetHashCode()
            {
                hash ??= HashCode.Combine(Key, Object);

                return hash.Value;
            }

            public readonly override string ToString()
            {
                if (this == default)
                    return StringHelper.EMPTY_OBJECT;

                return $"({nameof(Key)}: {Key}; {nameof(Object)}: {Object})";
            }
        }
    }
}
