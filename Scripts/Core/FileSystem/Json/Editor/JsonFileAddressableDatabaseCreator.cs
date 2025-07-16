using System;
using System.Linq;
using UnityEditor.AddressableAssets.Settings;
using UTIRLib.DB;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;
using UTIRLib.Json;

namespace UTIRLib.FileSystem.Json.Editor
{
    public static class JsonFileAddressableDatabaseCreator
    {
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CollectionArgumentException"></exception>
        /// <exception cref="StringArgumentException"></exception>
        public static void CreateDatabaseFile(AddressableAssetEntry[] addressableAssetEntries,
            FSPath directoryPath, string databaseName, bool overwrite)
        {
            if (addressableAssetEntries.IsNullOrEmpty())
            {
                throw new CollectionArgumentException(nameof(addressableAssetEntries),
                                                           addressableAssetEntries);
            }
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new StringArgumentException(nameof(databaseName));
            }

            AddressableAssetInfo[] convertedAddressables = addressableAssetEntries.Select(
                entry => new AddressableAssetInfo(entry)).ToArray();

            JsonFile dbFile = new(directoryPath, databaseName);

            dbFile.Serialize(convertedAddressables);

            dbFile.Save(overwrite);
        }
    }
}