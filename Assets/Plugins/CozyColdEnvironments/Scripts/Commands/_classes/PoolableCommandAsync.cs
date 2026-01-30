using CCEnvs.FuncLanguage;
using CCEnvs.Pools;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public abstract class PoolableCommandAsync : CommandAsync, IPoolable
    {
        private ReactiveCommand<IPoolable>? onDespawnCmd;

        protected Maybe<IDisposable> poolHandle => this.To<IPoolable>().PoolHandle;

        Maybe<IDisposable> IPoolable.PoolHandle { get; set; }

        protected PoolableCommandAsync(
            bool isSingle = false,
            string? name = null,
            int delayFrameCount = 0
            )
            :
            base(isSingle: isSingle,
                name: name,
                isResetable: true,
                delayFrameCount: delayFrameCount
                )
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
