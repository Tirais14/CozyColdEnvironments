using CCEnvs.FuncLanguage;
using CCEnvs.Pools;
using R3;
using System;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public abstract class PoolableCommand : Command, IPoolable
    {
        private ReactiveCommand<IPoolable>? onDespawnCmd;

        protected Maybe<IDisposable> poolHandle => this.To<IPoolable>().PoolHandle;

        Maybe<IDisposable> IPoolable.PoolHandle { get; set; }

        protected PoolableCommand()
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

        public virtual void Utilize()
        {
            if (!ReturnToPool())
                Dispose();
        }

        public bool ReturnToPool()
        {
            return poolHandle.Map(
                static x =>
                {
                    x.Dispose();
                    return true;
                })
                .GetValue(); ;
        }

        public Observable<IPoolable> ObserveDespawn()
        {
            onDespawnCmd ??= new ReactiveCommand<IPoolable>();

            return onDespawnCmd;
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
