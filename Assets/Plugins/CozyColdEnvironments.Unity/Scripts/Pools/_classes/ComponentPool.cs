using CCEnvs.Patterns.Factories;
using CCEnvs.Pools;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Pools
{
    public static class ComponentPool
    {
        public static void OnTransfomrReturn(Transform cmp)
        {
            var pos = new Vector3(0f, -100000f);

            cmp.MovePositionSafe(pos);
            cmp.gameObject.SetActive(false);
        }
    }

    public class ComponentPool<T> : ObjectPool<T>
        where T : Component
    {
        public ComponentPool(
            IFactory<T>? factory = null,
            int capacity = 4,
            int? maxSize = null
            )
            :
            base(factory: factory,
                capacity: capacity,
                maxSize: maxSize
                )
        {

        }

        protected override void OnGet(PooledObject<T> handledObj)
        {
            base.OnGet(handledObj);
            handledObj.Value.gameObject.SetActive(true);
        }

        protected override void OnReturn(T obj)
        {
            base.OnReturn(obj);
            ComponentPool.OnTransfomrReturn(obj.transform);
        }

        private int disposed;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (disposing)
            {
                if (fastObject.IsNotNull())
                    UnityEngine.Object.Destroy(fastObject.gameObject);

                while (inactiveItems.TryPop(out var cmp))
                    UnityEngine.Object.Destroy(cmp.gameObject);
            }
        }
    }
}
