using CCEnvs.FuncLanguage;
using CCEnvs.Pools;
using R3;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public abstract class PoolableCommandAsync : CommandAsync, IPoolable
    {
        private ReactiveCommand<IPoolable>? onDespawnCmd;

        protected Maybe<PooledObject> poolHandle {
            get => this.CastTo<IPoolable>().PoolHandle;
            set => this.CastTo<IPoolable>().PoolHandle = default;
        }

        Maybe<PooledObject> IPoolable.PoolHandle { get; set; }

        protected PoolableCommandAsync()
            :
            base(isResetable: true)
        {

        }

        public virtual void OnDespawned()
        {
            Reset();
            onDespawnCmd?.Execute(this);
        }

        public virtual void OnSpawned()
        {
        }

        public bool ReturnToPool()
        {
            if (!poolHandle.TryGetValue(out var handle))
                return false;

            handle.Dispose();
            return true;
        }

        public void Utilize()
        {
            if (!ReturnToPool())
                Dispose();
        }

        public Observable<IPoolable> ObserveDespawn()
        {
            onDespawnCmd ??= new ReactiveCommand<IPoolable>();

            return onDespawnCmd;
        }

        protected override void OnReset()
        {
            base.OnReset();

            poolHandle = default;
        }

        private bool disposed;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposed)
                return;

            poolHandle.IfSome(x => x.Dispose());
            onDespawnCmd?.Dispose();

            disposed = true;
        }
    }
}
