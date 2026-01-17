#nullable enable
using CCEnvs.FuncLanguage;
using R3;
using System;


namespace CCEnvs.Pools
{
    public interface IPoolable
    {
        Maybe<IDisposable> PoolHandle { get; set; }

        void OnDespawned();

        void OnSpawned();

        void OnSpawnedLate();

        Observable<IPoolable> ObserveDespawn();
    }
}
