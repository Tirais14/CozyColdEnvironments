using CCEnvs.Attributes.Serialization;
using CommunityToolkit.Diagnostics;
using ObservableCollections;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [TypeSerializationDescriptor("SaveArchive", "d619c03c-9b22-4be0-a351-e4cf2e66b4a0")]
    public class SaveArchive : IEnumerable<KeyValuePair<string, SaveGroupCatalog>>
    {
        private readonly ObservableDictionary<string, SaveGroupCatalog> catalogs = new(0);

        public IReadOnlyObservableDictionary<string, SaveGroupCatalog> Catalogs => catalogs;

        public string Path { get; }

        public SaveArchive(string? path = null)
        {
            Path = path ?? string.Empty;
        }

        //public void AddCatalog(SaveGroupCatalog catalog)
        //{
        //    Guard.IsNotNull(catalog, nameof(catalog));

        //    catalogs.Add(catalog.Path, catalog);
        //}

        public bool RemoveCatalog(string catalogPath, out SaveGroupCatalog? removed)
        {
            Guard.IsNotNull(catalogPath, nameof(catalogPath));

            return catalogs.Remove(catalogPath, out removed);
        }

        public SaveGroupCatalog GetOrCreateCatalog(string path)
        {
            Guard.IsNotNull(path, nameof(path));

            if (!catalogs.TryGetValue(path, out var catalog))
            {
                catalog = new SaveGroupCatalog(this, path);

                catalogs.Add(path, catalog);
            }

            return catalog;
        }

        public IEnumerator<KeyValuePair<string, SaveGroupCatalog>> GetEnumerator()
        {
            return catalogs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
