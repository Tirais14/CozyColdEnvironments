using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CCEnvs.Pools;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    [SerializationDescriptor("SaveGroupIncremental", "0c685a10-b6e3-4033-851b-8c740ffc7b59")]
    public sealed class SaveGroupIncremental : SaveGroup
    {
        [JsonIgnore]
        private readonly Dictionary<string, ISaveObjectIncremental> dirtyObjects = new();
        [JsonIgnore]
        private readonly Dictionary<string, object> nonIncrementalObjects = new();
        [JsonIgnore]
        private readonly Dictionary<string, OnSaveObjectIsDirtyChanged> incrementalObjectBindings = new();

        [JsonIgnore]
        private readonly IDisposable observableObjectAddBinding;
        [JsonIgnore]
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
            int entryCount;

            lock (SyncRoot)
                entryCount = dirtyObjects.Count + nonIncrementalObjects.Count;

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

                foreach (var (key, obj) in nonIncrementalObjects)
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

                lock (SyncRoot)
                {
                    foreach (var obj in observableObjects)
                    {
                        if (obj.Value is ISaveObjectIncremental saveObjectIncremental)
                            UnbindOnSaveObjectIsDirtyChanged(obj.Key, saveObjectIncremental);
                    }

                    dirtyObjects.Clear();
                    nonIncrementalObjects.Clear();
                }
            }

            disposed = true;

            base.Dispose(disposing);
        }

        private void ThrowIfDisposed()
        {
            if (!disposed)
                return;

            throw new ObjectDisposedException(GetType().Name);
        }

        private void OnSaveObjectIsDirtyChangedCore(
            string key,
            ISaveObjectIncremental obj,
            bool state
            )
        {
            ThrowIfDisposed();

            if (!state)
            {
                lock (SyncRoot)
                    dirtyObjects.Remove(key);

                return;
            }

            lock (SyncRoot)
                dirtyObjects.TryAdd(key, obj);
        }

        private void BindOnSaveObjectIsDirtyChanged(string key, ISaveObjectIncremental obj)
        {
            UnbindOnSaveObjectIsDirtyChanged(key, obj);

            obj.OnSaveObjectIsDirtyChanged += onSaveObjectIsDirtyChanged;

            lock (SyncRoot)
                incrementalObjectBindings[key] = onSaveObjectIsDirtyChanged;

            void onSaveObjectIsDirtyChanged(ISaveObjectIncremental obj, bool state)
            {
                OnSaveObjectIsDirtyChangedCore(key, obj, state);
            };
        }

        private void UnbindOnSaveObjectIsDirtyChanged(string key, ISaveObjectIncremental obj)
        {
            OnSaveObjectIsDirtyChanged binding;

            lock (SyncRoot)
            {
                if (!incrementalObjectBindings.Remove(key, out binding))
                    return;
            }

            obj.OnSaveObjectIsDirtyChanged -= binding;
        }

        private void OnObservableObjectAdd(DictionaryAddEvent<string, object> addEv)
        {
            ThrowIfDisposed();

            var obj = addEv.Value;
            var key = addEv.Key;

            if (obj is ISaveObjectIncremental saveObjectIncrementally)
                BindOnSaveObjectIsDirtyChanged(key, saveObjectIncrementally);
            else
            {
                lock (SyncRoot)
                    nonIncrementalObjects[key] = obj;
            }
        }

        private IDisposable BindObservableObjectAdd()
        {
            return observableObjects.ObserveDictionaryAdd(DisposeCancellationToken)
                .Subscribe(OnObservableObjectAdd);
        }

        private void OnObservableObjectRemove(DictionaryRemoveEvent<string, object> removeEv)
        {
            ThrowIfDisposed();

            var key = removeEv.Key;
            var obj = removeEv.Value;

            if (obj is ISaveObjectIncremental saveObjectIncrementally)
                UnbindOnSaveObjectIsDirtyChanged(key, saveObjectIncrementally);
            else
            {
                lock (SyncRoot)
                    nonIncrementalObjects.Remove(key);
            }
        }

        private IDisposable BindObservableObjectRemove()
        {
            return observableObjects.ObserveDictionaryRemove(DisposeCancellationToken)
                .Subscribe(OnObservableObjectRemove);
        }
    }
}
