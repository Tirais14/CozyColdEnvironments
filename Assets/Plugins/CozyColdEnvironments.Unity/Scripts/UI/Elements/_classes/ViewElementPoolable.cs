using CCEnvs.FuncLanguage;
using CCEnvs.Pools;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public abstract class ViewElementPoolable<TViewModel> : ViewElement<TViewModel>, IPoolable
        where TViewModel : IViewModel
    {
        Maybe<PooledObject> IPoolable.PoolHandle { get; set; }

        bool IPoolable.IsValid => !IsDestroyed;

        public bool ReturnToPool()
        {
            return ((IPoolable)this).PoolHandle.Map(x =>
                {
                    x.Dispose();
                    return true;
                })
                .GetValue();
        }

        public void Utilize()
        {
            if (!ReturnToPool())
                Destroy(gameObject);
        }

        protected virtual void OnPoolDespawned() { }

        protected virtual void OnPoolSpawned() { }

        void IPoolable.OnDespawned()
        {
            TrySetModel(null);
            OnPoolDespawned();
        }

        void IPoolable.OnSpawned()
        {
            OnPoolSpawned();
        }
    }
}
