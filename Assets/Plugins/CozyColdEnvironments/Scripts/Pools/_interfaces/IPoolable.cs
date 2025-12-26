#nullable enable
using R3;

namespace CCEnvs.Pools
{
    public interface IPoolable
    {
        void OnDespawned();

        void OnSpawned();

        void BindToPool(object pool);

        Observable<object> ObserveOnDespawned();
    }
}
