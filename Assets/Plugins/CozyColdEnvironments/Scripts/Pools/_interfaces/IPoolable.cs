#nullable enable
using CCEnvs.FuncLanguage;
using R3;

namespace CCEnvs.Pools
{
    public interface IPoolable : IUtilizable
    {
        Maybe<PooledObject> PoolHandle { get; set; }

        bool IsValid { get; }

        void OnDespawned();

        void OnSpawned();

        bool ReturnToPool();

        Observable<IPoolable> ObserveDespawn();
    }
}
