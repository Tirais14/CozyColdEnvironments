using CCEnvs.Patterns.Factories;
using CCEnvs.Pools;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Pools
{
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

            var pos = new Vector3(0f, -100000f);

            if (obj.gameObject.GetComponent<Rigidbody>().IsNotNull(out var rb))
                rb.MovePosition(pos);
            else
                obj.transform.position = pos;

            obj.gameObject.SetActive(false);
        }
    }
}
