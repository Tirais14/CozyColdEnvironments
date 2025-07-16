using System;
using UTIRLib.Diagnostics;

#nullable enable

namespace UTIRLib.DB
{
    public record DatabaseFileInfo
    {
        private readonly string databaseFilePath;
        private readonly string databaseName;
        private readonly bool loadAssetsImmediately;
        private readonly AssetType databaseAssetType;
        public string DatabaseFilePath => databaseFilePath;
        public string DatabaseName => databaseName;
        public bool LoadAssetsImmediately => loadAssetsImmediately;
        public AssetType DatabaseAssetType => databaseAssetType;

        /// <exception cref="NullOrEmptyStringException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public DatabaseFileInfo(string databaseFilePath, string databaseName, AssetType databaseAssetType,
            bool loadAssetsImmediately = false)
        {
            if (string.IsNullOrEmpty(databaseFilePath))
            {
                throw new StringArgumentException(databaseFilePath, nameof(databaseFilePath));
            }
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new StringArgumentException(databaseName, nameof(databaseName));
            }
            if (databaseAssetType == AssetType.None)
            {
                throw new ArgumentException($"Value {nameof(databaseFilePath)} cannot contain {AssetType.None}");
            }

            this.databaseFilePath = databaseFilePath;
            this.databaseName = databaseName;
            this.loadAssetsImmediately = loadAssetsImmediately;
            this.databaseAssetType = databaseAssetType;
        }
    }
}