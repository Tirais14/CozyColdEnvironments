using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.UnityX.Databases
{
    public interface IAssetDatabaseRegistry
        : IDictionary<Identifier, IAssetDatabase>,
        IDisposable
    {
        AssetDatabaseQuery Query();
    }
}
