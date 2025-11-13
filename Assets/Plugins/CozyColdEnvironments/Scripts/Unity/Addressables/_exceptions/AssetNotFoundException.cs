using CCEnvs.Diagnostics;
using CCEnvs.Unity.AddrsAssets.Databases;
using System;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public class AssetNotFoundException : CCException
    {
        public AssetNotFoundException()
        {
        }

        public AssetNotFoundException(IAddressablesDatabase? db, Identifier id, Type? type)
            : 
            base(Sentence.Empty.AddIfNotDefault(() => $"Database: {db},...", db)
                .Add($"ID: {id},...")
                .AddIfNotDefault(() => $"type: {type},...", type)
                .ToString())
        {
        }
    }
}
