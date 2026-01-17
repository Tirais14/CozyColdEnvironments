using CommunityToolkit.Diagnostics;
using System;

#nullable enable
#pragma warning disable S927
namespace CCEnvs.Pools
{
    public class ObjectPoolProxi<TPool, TObject> : IObjectPool<TObject>
        where TObject : class, IPoolable
    {
        private readonly TPool pool;
        private readonly Func<TPool, PooledHandle<TObject>> getAction;
        private readonly Action<TPool, TObject> releaseAction;

        public ObjectPoolProxi(TPool pool,
            Func<TPool, PooledHandle<TObject>> getAction,
            Action<TPool, TObject> releaseAction)
        {
            CC.Guard.IsNotNull(pool, nameof(pool));
            Guard.IsNotNull(getAction, nameof(getAction));
            Guard.IsNotNull(releaseAction, nameof(releaseAction));

            this.pool = pool;
            this.getAction = getAction;
            this.releaseAction = releaseAction;
        }

        public virtual PooledHandle<TObject> Get()
        {
            return getAction(pool);
        }

        public virtual void Release(TObject obj)
        {
            releaseAction(pool, obj);
        }
    }
}
