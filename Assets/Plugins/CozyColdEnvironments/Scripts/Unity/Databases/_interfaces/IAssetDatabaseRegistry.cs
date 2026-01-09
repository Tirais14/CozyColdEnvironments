using CCEnvs.Collections;
using System;

#nullable enable
namespace CCEnvs.Unity.Databases
{
    public interface IAssetDatabaseRegistry 
        : ICCDictionary<Identifier, IAddressablesDatabase>,
        IDisposable
    {
        AssetDatabaseQuery Query();
    }
}
