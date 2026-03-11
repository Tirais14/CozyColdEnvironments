using CCEnvs.Attributes;
using CommunityToolkit.Diagnostics;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Saves
{
    public static class SaveSystemSerializedStorage
    {
        private readonly static Dictionary<string, SaveArchiveSerialized> archives = new();

        public static IReadOnlyDictionary<string, SaveArchiveSerialized> Archives => archives;

        public static object ArchivesGate { get; } = new();

        public static void AddArchive(SaveArchiveSerialized archive)
        {
            Guard.IsNotDefault(archive, nameof(archive));

            lock (ArchivesGate)
                archives[archive.Path] = archive;
        }

        public static bool RemoveArchive(string path)
        {
            Guard.IsNotNullOrWhiteSpace(path, nameof(path));

            lock (ArchivesGate)
                return archives.Remove(path);
        }  

        public static void AddCatalog(
            string archivePath,
            SaveCatalogSerialized catalog
            )
        {
            Guard.IsNotNullOrWhiteSpace(archivePath, nameof(archivePath));
            Guard.IsNotDefault(catalog, nameof(catalog));

            SaveArchiveSerialized archive;

            lock (ArchivesGate)
            {
                if (!archives.TryGetValue(archivePath, out archive))
                {
                    archive = new SaveArchiveSerialized(archivePath);
                    archives.Add(archivePath, archive);
                }
            }

            archive.Add(catalog);
        }

        public static bool RemoveCatalog(string archivePath, string catalogPath)
        {
            Guard.IsNotNullOrWhiteSpace(archivePath, nameof(archivePath));
            Guard.IsNotNullOrWhiteSpace(catalogPath, nameof(catalogPath));

            SaveArchiveSerialized archive;

            lock (ArchivesGate)
            {
                if (!archives.TryGetValue(archivePath, out archive))
                    return false;
            }

            return archive.Remove(catalogPath);
        }

        public static void AddGroup(
            string archivePath, 
            string catalogPath,
            SaveGroupSerialized group)
        {
            Guard.IsNotNullOrWhiteSpace(archivePath, nameof(archivePath));
            Guard.IsNotNullOrWhiteSpace(catalogPath, nameof(catalogPath));
            Guard.IsNotDefault(group, nameof(group));

            SaveArchiveSerialized archive;

            lock (ArchivesGate)
            {
                if (!archives.TryGetValue(archivePath, out archive))
                {
                    archive = new SaveArchiveSerialized(archivePath);
                    archives.Add(archivePath, archive);
                }
            }

            SaveCatalogSerialized catalog;

            lock (archive.CatalogsGate)
            {
                if (!archive.Catalogs.TryGetValue(catalogPath, out catalog))
                {
                    catalog = new SaveCatalogSerialized(catalogPath);
                    archive.Add(catalog);
                }
            }

            catalog.Add(group);
        }

        public static bool RemoveGroup(
            string archivePath,
            string catalogPath,
            string groupName
            )
        {
            Guard.IsNotNullOrWhiteSpace(archivePath, nameof(archivePath));
            Guard.IsNotNullOrWhiteSpace(catalogPath, nameof(catalogPath));
            Guard.IsNotNull(groupName, nameof(groupName));

            SaveArchiveSerialized archive;

            lock (ArchivesGate)
            {
                if (!archives.TryGetValue(archivePath, out archive))
                    return false;
            }

            SaveCatalogSerialized catalog;

            lock (archive.CatalogsGate)
            {
                if (!archive.Catalogs.TryGetValue(catalogPath, out catalog))
                    return false;
            }

            return catalog.Remove(groupName);
        }

        [OnInstallExecutable]
        public static void ClearArchives()
        {
            lock (ArchivesGate)
                archives.Clear();
        }

        public static void ClearCatalogs()
        {
            lock (ArchivesGate)
            {
                foreach (var archive in archives.Values)
                    archive.Clear();
            }
        }

        public static void ClearGroups()
        {
            lock (ArchivesGate)
            {
                foreach (var archive in archives.Values)
                {
                    lock (archive.CatalogsGate)
                    {
                        foreach (var catalog in archive.Catalogs.Values)
                            catalog.Clear();
                    }
                }
            }
        }
    }
}
