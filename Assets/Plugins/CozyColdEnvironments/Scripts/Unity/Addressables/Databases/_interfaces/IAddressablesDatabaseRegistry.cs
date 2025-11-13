using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public interface IAddressablesDatabaseRegistry 
        : IDictionary<Identifier, IAddressablesDatabase>,
        IDisposable,
        ILoadable
    {
        AddressablesDatabaseSearch Get();
    }
}
