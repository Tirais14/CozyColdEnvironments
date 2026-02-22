using CCEnvs.Attributes.Serialization;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using ObservableCollections;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    [TypeSerializationDescriptor("Saves.SaveArchive", "d619c03c-9b22-4be0-a351-e4cf2e66b4a0")]
    public class SaveArchive : IEquatable<SaveArchive>, IEnumerable<KeyValuePair<string, SaveCatalog>>
    {
        private readonly ObservableDictionary<string, SaveCatalog> catalogs = new();

        private int? hashCode;

        [JsonIgnore]
        public IReadOnlyObservableDictionary<string, SaveCatalog> Catalogs => catalogs;

        [JsonProperty("path")]
        public string Path { get; }

        [JsonConstructor]
        public SaveArchive(string? path = null)
        {
            Path = path ?? string.Empty;
        }

        public static bool operator ==(SaveArchive? left, SaveArchive? right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left is null || right is null) 
                return false;

            return left.Path == right.Path;
        }

        public static bool operator !=(SaveArchive? left, SaveArchive? right)
        {
            return !(left == right);
        }

        public bool RemoveCatalog(string catalogPath, out SaveCatalog? removed)
        {
            Guard.IsNotNull(catalogPath, nameof(catalogPath));

            return catalogs.Remove(catalogPath, out removed);
        }

        public SaveCatalog GetOrCreateCatalog(string path)
        {
            Guard.IsNotNull(path, nameof(path));

            if (!catalogs.TryGetValue(path, out var catalog))
            {
                catalog = new SaveCatalog(this, path);

                catalogs.Add(path, catalog);
            }

            return catalog;
        }

        public bool Equals(SaveArchive other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is SaveArchive typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(Path);

            return hashCode.Value;  
        }

        public override string ToString()
        {
            return $"({nameof(Path)}: {Path})";
        }

        public IEnumerator<KeyValuePair<string, SaveCatalog>> GetEnumerator()
        {
            return catalogs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
