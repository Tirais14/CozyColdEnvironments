#nullable enable
using CCEnvs.Collections;
using CCEnvs.Patterns.Factories;
using System.Buffers;

namespace CCEnvs.Pools  
{
    public class ObjectPoolAsync<T> : AObjectPool<T>, IObjectPoolAsync<T>
        where T : class
    {
        private readonly IFactory<
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<T>
#else
        System.Threading.Tasks.Task<T>
#endif
            >? factory;

        public override bool HasFactory => factory is not null;

        public ObjectPoolAsync(

            IFactory<
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<T>
#else
        System.Threading.Tasks.Task<T>
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
        System.Threading.Tasks.Task<PooledHandle<T>>
#endif
            GetAsync()
        {
            T obj;

            if (FastObject is not null)
                obj = FastObject;
            else if (InactiveCount == 0)
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
        System.Threading.Tasks.Task
#endif
            PreheatAsync(int? count = null)
        {
            int resolvedCount = (count ?? DefaultCapacity) - Count;

            var tasksHandle = ArrayPool<
#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask<PooledHandle<T>>
#else
            System.Threading.Tasks.Task<T>
#endif
            >.Shared.RentHandled(resolvedCount, resolvedCount);

#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask<PooledHandle<T>>
#else
            System.Threading.Tasks.Task<T>
#endif
                task;

            for (int i = 0; i < resolvedCount; i++)
            {
                task = GetAsync();

                tasksHandle.Value.AddToArraySegment(task);
            }

            await
#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask
#else
            System.Threading.Tasks.Task
#endif
                .WhenAll(tasksHandle.Value.ToArray());

            PooledHandle<T> handle;

            for (int i = 0; i < resolvedCount; i++)
            {
                handle = await tasksHandle.Value[i];
                handle.Dispose();
            }
        }
    }
}
