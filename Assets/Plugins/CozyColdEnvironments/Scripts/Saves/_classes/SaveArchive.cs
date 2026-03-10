using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Linq;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using ValueTaskSupplement;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Saves
{
    public sealed class SaveArchive
        :
        IEnumerable<SaveCatalog>,
        IDisposable
    {
        public const string DEFAULT_PATH = "Default";

        private ObservableDictionary<string, SaveCatalog> catalogs = new();

        private int? hashCode;

        public IReadOnlyObservableDictionary<string, SaveCatalog> Catalogs => catalogs;

        public string Path { get; init; }

        public SaveArchive(string? path = null)
        {
            Path = path ?? DEFAULT_PATH;
        }

        ~SaveArchive() => Dispose();

        public bool RemoveCatalog(string catalogPath)
        {
            Guard.IsNotNull(catalogPath, nameof(catalogPath));

            if (!catalogs.Remove(catalogPath, out var catalog))
                return false;

            catalog.Dispose();
            return true;
        }

        public SaveCatalog GetOrCreateCatalog(string? path = null)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (path.IsNullOrWhiteSpace())
                path = DEFAULT_PATH;

            SaveCatalog? catalog;

            lock (catalogs.SyncRoot)
            {
                if (!catalogs.TryGetValue(path, out catalog))
                {
                    catalog = new SaveCatalog(this, path);

                    catalogs.Add(path, catalog);
                }
            }

            return catalog;
        }

        public SaveArchive Clear()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            lock (catalogs.SyncRoot)
                catalogs.SelectValue().DisposeEach(bufferized: false);

            catalogs.Clear();

            return this;
        }

        public override string ToString()
        {
            return $"({nameof(Path)}: {Path})";
        }

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            lock (catalogs.SyncRoot)
                catalogs.SelectValue().DisposeEach(bufferized: false);

            catalogs.Clear();
        }

        public IEnumerator<SaveCatalog> GetEnumerator()
        {
            return catalogs.To<IDictionary<string, SaveCatalog>>().Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
