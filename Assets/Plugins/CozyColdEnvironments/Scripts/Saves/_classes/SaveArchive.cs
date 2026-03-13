using CCEnvs.Disposables;
using CCEnvs.Linq;
using CCEnvs.Patterns.Commands;
using CommunityToolkit.Diagnostics;
using ObservableCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

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

        internal readonly CommandScheduler commandScheduler = CommandScheduler.CreateDefaultRegistered(nameof(SaveArchive));

        private ObservableDictionary<string, SaveCatalog> catalogs = new();

        private int? hashCode;

        public IReadOnlyObservableDictionary<string, SaveCatalog> Catalogs => catalogs;

        public string Path { get; }

        public SaveArchiveLoader Loader { get; }

        public SaveArchiveSerializer Serializer { get; }

        public SaveArchive(string? path = null)
        {
            Path = path ?? DEFAULT_PATH;
            Loader = new SaveArchiveLoader(this);
            Serializer = new SaveArchiveSerializer(this);
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

            Loader.Dispose();
            commandScheduler.Dispose();
            Clear();

            GC.SuppressFinalize(this);
        }

        public IEnumerator<SaveCatalog> GetEnumerator()
        {
            return catalogs.To<IDictionary<string, SaveCatalog>>().Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
