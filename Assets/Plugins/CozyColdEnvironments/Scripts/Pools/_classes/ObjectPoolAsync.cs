#nullable enable
using CCEnvs.Patterns.Factories;
using System.Threading.Tasks;

namespace CCEnvs.Pools  
{
    public class ObjectPoolAsync<T> : AObjectPool<T>, IObjectPoolAsync<T>
        where T : class
    {
        private readonly IFactory<
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<T>
#else
        System.Threading.Tasks.ValueTask<T>
#endif
            >? factory;

        public override bool HasFactory => factory is not null;

        public ObjectPoolAsync(

            IFactory<
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
            GetAsync()
        {
            T obj;

            if (FastObject is not null)
                obj = FastObject;
            else if (InactiveCount <= 0)
            {
                if (factory is null)
                    throw IsEmptyException();

                obj = await factory.Create();
            }
            else
                obj = inactiveItems.Dequeue();

            var handle = CreateHandle(obj);
            OnGet(handle);

            return handle;
        }

        public async
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask
#else
        System.Threading.Tasks.ValueTask
#endif
            PreheatAsync(int? count = null)
        {
            int resolvedCount = (count ?? DefaultCapacity) - Count;
            PooledHandle<T> handle;

            for (int i = 0; i < resolvedCount; i++)
            {
                handle = await GetAsync();
                handle.Dispose();
            }
        }
    }
}
