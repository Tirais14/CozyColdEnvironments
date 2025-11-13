using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Unity.AddrsAssets.Databases;
using System;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public class DatabaseNotFoundException : CCException
    {
        public DatabaseNotFoundException(Identifier id,
            IAddressablesDatabaseRegistry? reg = null,
            Type? assetType = null)
            :
            this(Sentence.Empty.Add($"ID: {id},...")
                .AddIfNotDefault(() => $"Registry: {reg},...", reg)
                .AddIfNotDefault(() => $"Asset type: {assetType.GetFullName()}...", assetType)
                .ToString()))
        {
        }
    }
}
