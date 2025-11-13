using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using CCEnvs.Unity.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using ZLinq;

#nullable enable
#pragma warning disable S3881
#pragma warning disable IDE1006
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public abstract class AddressablesDatabaseRegistry<TThis>
        : CCBehaviourStaticPublic<TThis>,
        IAddressablesDatabaseRegistry

        where TThis : CCBehaviourStatic, IAddressablesDatabaseRegistry
    {
        private readonly CCDictionary<Identifier, IAddressablesDatabase> collection = new();
        private readonly AddressablesDatabaseSearch search = new();

        private Stopwatch? stopwatch;

        public event Action? OnStartLoading;
        public event Action? OnLoaded;

        public Result<IAddressablesDatabase> this[Identifier key] {
            get => collection[key];
            set => collection[key] = value;
        }

        public IEnumerable<Identifier> Keys => collection.Keys;
        public IEnumerable<IAddressablesDatabase> Values => collection.Values;
        public int Count => collection.Count;
        public bool IsLoading => collection.Values.Any(db => db.IsLoading);
        public bool IsLoaded => !IsLoading && Count > 0;

        bool ICollection<KeyValuePair<Identifier, IAddressablesDatabase>>.IsReadOnly => false;

        protected override void Awake()
        {
            base.Awake();

            BindEvents();
        }

        public AddressablesDatabaseSearch Search()
        {
            return search.Reset().From(this);
        }

        public void Add(Identifier key, IAddressablesDatabase value)
        {
            collection.Add(key, value);
        }

        public void Add(KeyValuePair<Identifier, IAddressablesDatabase> item)
        {
            collection.Add(item);
        }

        public bool Remove(Identifier key)
        {
            return collection.Remove(key);
        }

        public bool Remove(KeyValuePair<Identifier, IAddressablesDatabase> item)
        {
            return collection.Remove(item.Key);
        }

        public bool ContainsKey(Identifier key)
        {
            return collection.ContainsKey(key);
        }

        public bool Contains(KeyValuePair<Identifier, IAddressablesDatabase> item)
        {
            return collection.Contains(item);
        }

        public void CopyTo(KeyValuePair<Identifier, IAddressablesDatabase>[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        public void Clear()
        {
            collection.Clear();
        }

        public void Dispose() => Dispose(true);

        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                collection.Values.CForEach(x => x.Dispose());
                collection.Clear();
            }

            disposed = true;
        }

        public IEnumerator<KeyValuePair<Identifier, IAddressablesDatabase>> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected void BindEvents()
        {
            OnStartLoading += () =>
            {
                stopwatch ??= new Stopwatch();
                stopwatch.Start();
                this.PrintLog("Loading started");
            };

            OnLoaded += () =>
            {
                stopwatch!.Stop();
                this.PrintLog($"Loading finished in {stopwatch.Elapsed.TotalSeconds} seconds.");
            };
        }
    }
}
