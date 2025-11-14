using CCEnvs.Collections;
using System;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public interface IAddressablesDatabaseRegistry 
        : ICCDictionary<Identifier, IAddressablesDatabase>,
        IDisposable
    {
        AddressablesDatabaseSearch Search();
    }
}
