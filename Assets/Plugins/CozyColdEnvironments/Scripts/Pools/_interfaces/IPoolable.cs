#nullable enable
using CCEnvs.FuncLanguage;
using R3;
using System;

namespace CCEnvs.Pools
{
    public interface IPoolable : IUtilizable
    {
        event Action<IPoolable> OnDespawnCallback;

        Maybe<PooledObject> PoolHandle { get; set; }

        bool IsValid { get; }

        void OnDespawned();

        void OnSpawned();

        bool ReturnToPool();
    }
}
