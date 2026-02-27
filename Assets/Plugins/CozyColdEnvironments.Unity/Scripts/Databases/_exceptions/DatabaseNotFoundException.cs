using System;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;

#nullable enable
namespace CCEnvs.Unity.Databases
{
    public class DatabaseNotFoundException : CCException
    {
        public DatabaseNotFoundException(Identifier? id,
            IAssetDatabaseRegistry? reg = null,
            Type? assetType = null)
            :
            base(Sentence.Empty.Add($"ID: {id},...")
                .AddIfNotDefault(() => $"Registry: {reg},...", reg)
                .AddIfNotDefault(() => $"Asset type: {assetType.GetFullName()}...", assetType)
                .ToString())
        {
        }
    }
}
