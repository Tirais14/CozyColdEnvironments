#nullable enable
using CCEnvs.Patterns.Factories;
using System.Threading;

namespace CCEnvs.Pools  
{
    public class ObjectPoolAsync<T> : ObjectPoolBase<T>, IObjectPoolAsync<T>
        where T : class
    {
        private readonly IFactory<CancellationToken,
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<T>
#else
        System.Threading.Tasks.ValueTask<T>
#endif
            >? factory;

        public override bool HasFactory => factory is not null;

        public ObjectPoolAsync(

            IFactory<CancellationToken,
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<T>
#else
        System.Threading.Tasks.ValueTask<T>
#endif
                >? factory = null,

            int capacity = 4,
            int? maxSize = null)
            :
            base(capacity, maxSize)
        {
            this.factory = factory;
        }

        public async
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<PooledHandle<T>>
#else
        System.Threading.Tasks.ValueTask<PooledHandle<T>>
#endif
            GetAsync(CancellationToken cancellationToken = default)
        {
            T? obj = null;

            while (!IsObjectValid(obj))
            {
                if (InactiveCount < 1)
                {
                    if (factory is null)
                        throw IsEmptyException();

                    obj = await factory.Create(cancellationToken);
                    Return(obj);
                }

                if (TryGetFromInactive(out obj))
                    break;
            }

            var handle = CreateHandle(obj);
            GetCore(handle);
            OnGet(handle);

            return handle;
        }
    }
}
