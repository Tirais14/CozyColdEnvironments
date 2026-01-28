#nullable enable
using CCEnvs.FuncLanguage;
using R3;
using System;


namespace CCEnvs.Pools
{
    public interface IPoolable
    {
        Maybe<IDisposable> PoolHandle { get; set; }

        bool IsValid { get; }

        void OnDespawned();

        void OnSpawned();

        Observable<IPoolable> ObserveDespawn();
    }
}
