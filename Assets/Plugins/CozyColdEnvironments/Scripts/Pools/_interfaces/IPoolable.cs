#nullable enable
using CCEnvs.FuncLanguage;

namespace CCEnvs.Pools
{
    public interface IPoolable : IUtilizable
    {
        Maybe<PooledObject> PoolHandle { get; set; }

        bool IsValid { get; }

        void OnDespawned();

        void OnSpawned();

        bool ReturnToPool();
    }
}
