#nullable enable
using R3;
using System;

namespace CCEnvs.Pools
{
    public interface IPoolable
    {
        void OnDespawned();

        void OnSpawned();

        void BindPoolHandle(IDisposable handle);
    }
}
