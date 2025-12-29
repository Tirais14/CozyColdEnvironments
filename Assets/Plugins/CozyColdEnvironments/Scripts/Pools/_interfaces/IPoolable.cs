#nullable enable
using CCEnvs.FuncLanguage;
using R3;
using System;


namespace CCEnvs.Pools
{
    public interface IPoolable
    {
        Maybe<IDisposable> PoolHandle { get; }

        void OnDespawned();

        void OnSpawned();

        void OnSpawnedLate();

        void BindPoolHandle(IDisposable handle);

        Observable<IPoolable> ObserveDespawn();
    }
}
