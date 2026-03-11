using CCEnvs.Linq;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Saves
{
    [Serializable]
    public readonly struct SaveArchiveSerialized : IEquatable<SaveArchiveSerialized>
    {
        private readonly Dictionary<string, SaveCatalogSerialized> catalogs;

        public IReadOnlyDictionary<string, SaveCatalogSerialized> Catalogs => catalogs;

        public string Path { get; }

        public object CatalogsGate { get; }

        [JsonConstructor]
        public SaveArchiveSerialized(
            string path,
            IEnumerable<SaveCatalogSerialized>? catalogs = null
            )
        {
            Guard.IsNotNullOrWhiteSpace(path, nameof(path));
            CC.Guard.IsNotNull(catalogs, nameof(catalogs));

            Path = path;
            CatalogsGate = new object();

            catalogs ??= Array.Empty<SaveCatalogSerialized>();

            var catalogDic = new Dictionary<string, SaveCatalogSerialized>();

            if (catalogs.TryGetNonEnumeratedCount(out var count))
                catalogDic.EnsureCapacity(count);

            foreach (var catalog in catalogs)
                catalogDic.Add(catalog.Path, catalog);

            this.catalogs = catalogDic;
        }

        public static bool operator ==(SaveArchiveSerialized left, SaveArchiveSerialized right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveArchiveSerialized left, SaveArchiveSerialized right)
        {
            return !(left == right);
        }

        public void Add(SaveCatalogSerialized catalog)
        {
            if (this == default)
                throw new InvalidOperationException($"{this} is not initialized");

            Guard.IsNotDefault(catalog, nameof(catalog));

            lock (CatalogsGate)
                catalogs[catalog.Path] = catalog;
        }

        public bool Remove(string catalogPath)
        {
            if (this == default)
                throw new InvalidOperationException($"{this} is not initialized");

            Guard.IsNotNullOrWhiteSpace(catalogPath, nameof(catalogPath));

            lock (CatalogsGate)
                return catalogs.Remove(catalogPath);
        }

        public void Clear()
        {
            lock (CatalogsGate)
                catalogs.Clear();
        }

        public override bool Equals(object? obj)
        {
            return obj is SaveArchiveSerialized serialized && Equals(serialized);
        }

        public bool Equals(SaveArchiveSerialized other)
        {
            return catalogs == other.catalogs
                   &&
                   Path == other.Path;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(catalogs, Path);
        }
    }
}
