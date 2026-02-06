using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Databases
{
    public interface IAssetDatabaseRegistry 
        : IDictionary<Identifier, IAssetDatabase>,
        IDisposable
    {
        AssetDatabaseQuery Query();
    }
}
