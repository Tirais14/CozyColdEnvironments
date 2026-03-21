using CCEnvs.FuncLanguage;
using CCEnvs.Pools;
using System;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public abstract class PoolableCommandAsync : CommandAsync, IPoolable
    {
        protected Maybe<PooledObject> poolHandle {
            get => this.CastTo<IPoolable>().PoolHandle;
            set => this.CastTo<IPoolable>().PoolHandle = default;
        }

        Maybe<PooledObject> IPoolable.PoolHandle { get; set; }

        public event Action<IPoolable>? OnDespawnCallback;

        protected PoolableCommandAsync()
            :
            base(isResetable: true)
        {

        }

        public virtual void OnDespawned()
        {
            Reset();
            OnDespawnCallback?.Invoke(this);
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

            disposed = true;
        }
    }
}
