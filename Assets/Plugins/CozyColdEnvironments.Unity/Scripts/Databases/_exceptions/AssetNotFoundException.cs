using CCEnvs.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Unity.Databases
{
    public class AssetNotFoundException : CCException
    {
        public AssetNotFoundException()
        {
        }

        public AssetNotFoundException(IAssetDatabase? db, Identifier? id, Type? type)
            :
            base(Sentence.Empty.AddIfNotDefault(() => $"Database: {db},...", db)
                .Add($"ID: {id},...")
                .AddIfNotDefault(() => $"type: {type},...", type)
                .ToString())
        {
        }
    }
}
