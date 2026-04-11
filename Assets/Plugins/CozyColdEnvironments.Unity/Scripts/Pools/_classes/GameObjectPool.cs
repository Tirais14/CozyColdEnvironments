using CCEnvs.Patterns.Factories;
using CCEnvs.Pools;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Pools
{
    public class GameObjectPool : ObjectPool<GameObject>
    {
        public GameObjectPool(
            IFactory<GameObject>? factory = null,
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

        protected override void OnGet(PooledObject<GameObject> handledObj)
        {
            base.OnGet(handledObj);
            handledObj.Value.SetActive(true);
        }

        protected override void OnReturn(GameObject obj)
        {
            base.OnReturn(obj);
            var pos = new Vector3(0f, -100000f);

            obj.transform.MovePositionSafe(pos);
            obj.SetActive(false);
        }
    }
}
