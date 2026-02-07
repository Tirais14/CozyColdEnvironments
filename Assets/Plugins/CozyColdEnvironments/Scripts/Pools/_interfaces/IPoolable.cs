#nullable enable
using CCEnvs.FuncLanguage;
using R3;
using System;

namespace CCEnvs.Pools
{
    public interface IPoolable : IUtilizable
    {
        Maybe<IDisposable> PoolHandle { get; set; }

        bool IsValid { get; }

        //bool IsReady { get; }

        void OnDespawned();

        void OnSpawned();

        bool ReturnToPool();

        Observable<IPoolable> ObserveDespawn();
    }
}
