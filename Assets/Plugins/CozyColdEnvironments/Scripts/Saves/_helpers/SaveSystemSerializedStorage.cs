using CommunityToolkit.Diagnostics;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Saves
{
    public static class SaveSystemSerializedStorage
    {
        private static Dictionary<string, SaveArchiveSerialized> archives = new();

        public static IReadOnlyDictionary<string, SaveArchiveSerialized> Archives => archives;

        public static void Add(SaveArchiveSerialized archive)
        {
            Guard.IsNotDefault(archive, nameof(archive));

            archives[archive.Path] = archive;
        }

        public static bool Remove(string path)
        {
            Guard.IsNotNullOrWhiteSpace(path, nameof(path));

            return archives.Remove(path);
        }  

        public static void Clear()
        {
            archives.Clear();
        }
    }
}
