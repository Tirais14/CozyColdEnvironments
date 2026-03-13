using System;
using System.Collections.Generic;
using System.Threading;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Pools;
using ObservableCollections;
using R3;

#nullable enable
namespace CCEnvs.Saves
{
    [SerializationDescriptor("SaveGroupIncremental", "0c685a10-b6e3-4033-851b-8c740ffc7b59")]
    public sealed class SaveGroupIncremental : SaveGroup
    {
        private readonly Dictionary<string, ISaveObjectIncremental> dirtyObjects = new();
        private readonly Dictionary<string, object> nonIncrementalObjects = new();
        private readonly Dictionary<string, OnSaveObjectIsDirtyChanged> incrementalObjectBindings = new();

        private readonly object dirtyObjectGate = new();
        private readonly object nonIncrementalObjectsGate = new();
        private readonly object incrementalObjectBindingsGate = new();

        private readonly IDisposable observableObjectAddBinding;
        private readonly IDisposable observableObjectRemoveBinding;

        public bool HasDirtyObjects {
            get
            {
                lock (nonIncrementalObjectsGate)
                {
                    if (nonIncrementalObjects.Count != 0)
                        return true;
                }

                lock (dirtyObjectGate)
                    return dirtyObjects.Count != 0;
            }
        }

        public int DirtyObjectCount {
            get
            {
                lock (dirtyObjectGate)
                    lock (nonIncrementalObjectsGate)
                        return dirtyObjects.Count + nonIncrementalObjects.Count;
            }
        }

        public SaveGroupIncremental(
            SaveCatalog catalog,
            string? name = null,
            long saveDataVersion = 0L,
            RedirectionMode redirectionMode = default
            )
            :
            base(catalog, name, saveDataVersion, redirectionMode)
        {
            observableObjectAddBinding = BindObservableObjectAdd();
            observableObjectRemoveBinding = BindObservableObjectRemove();
        }

        internal override PooledObject<List<SaveEntry>> CreateAndProcessSaveEntriesPooled()
        {
            int entryCount;

            lock (dirtyObjectGate)
                lock (nonIncrementalObjectsGate)
                    entryCount = dirtyObjects.Count + nonIncrementalObjects.Count;

            var saveEntries = ListPool<SaveEntry>.Shared.Get();

            saveEntries.Value.TryIncreaseCapacity(entryCount);

            lock (dirtyObjectGate)
            {
                foreach (var (key, obj) in dirtyObjects)
                {
                    if (!TryCreateAndProcessSaveEntry(key, obj, out var saveEntry))
                        continue;

                    saveEntries.Value.Add(saveEntry);
                }
            }

            lock (nonIncrementalObjectsGate)
            {
                foreach (var (key, obj) in nonIncrementalObjects)
                {
                    if (!TryCreateAndProcessSaveEntry(key, obj, out var saveEntry))
                        continue;

                    saveEntries.Value.Add(saveEntry);
                }
            }

            return saveEntries;
        }

        private int disposed;
        protected override void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (disposing)
            {
                observableObjectAddBinding?.Dispose();
                observableObjectRemoveBinding?.Dispose();

                lock (observableObjects.SyncRoot)
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

            base.Dispose(disposing);
        }

        private void OnSaveObjectIsDirtyChangedCore(
            string key,
            ISaveObjectIncremental obj,
            bool state
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (!state)
            {
                lock (dirtyObjectGate)
                    dirtyObjects.Remove(key);

                return;
            }

            lock (dirtyObjectGate)
                dirtyObjects.TryAdd(key, obj);
        }

        private void BindOnSaveObjectIsDirtyChanged(string key, ISaveObjectIncremental obj)
        {
            UnbindOnSaveObjectIsDirtyChanged(key, obj);

            obj.OnSaveObjectIsDirtyChanged += onSaveObjectIsDirtyChanged;

            lock (incrementalObjectBindingsGate)
                incrementalObjectBindings[key] = onSaveObjectIsDirtyChanged;

            void onSaveObjectIsDirtyChanged(ISaveObjectIncremental obj, bool state)
            {
                OnSaveObjectIsDirtyChangedCore(key, obj, state);
            }
            ;
        }

        private void UnbindOnSaveObjectIsDirtyChanged(string key, ISaveObjectIncremental obj)
        {
            OnSaveObjectIsDirtyChanged binding;

            lock (incrementalObjectBindingsGate)
            {
                if (!incrementalObjectBindings.Remove(key, out binding))
                    return;
            }

            obj.OnSaveObjectIsDirtyChanged -= binding;
        }

        private void OnObservableObjectAdd(DictionaryAddEvent<string, object> addEv)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            var obj = addEv.Value;
            var key = addEv.Key;

            if (obj is ISaveObjectIncremental saveObjectIncrementally)
                BindOnSaveObjectIsDirtyChanged(key, saveObjectIncrementally);
            else
            {
                lock (nonIncrementalObjectsGate)
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
            CCDisposable.ThrowIfDisposed(this, disposed);

            var key = removeEv.Key;
            var obj = removeEv.Value;

            if (obj is ISaveObjectIncremental saveObjectIncrementally)
                UnbindOnSaveObjectIsDirtyChanged(key, saveObjectIncrementally);
            else
            {
                lock (nonIncrementalObjectsGate)
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
