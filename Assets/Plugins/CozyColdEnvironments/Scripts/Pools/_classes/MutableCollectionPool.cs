using System.Collections.Generic;
using CCEnvs.Patterns.Factories;

#nullable enable
namespace CCEnvs.Pools
{
    public class MutableCollectionPool<TCollection, TItem> : ObjectPool<TCollection>
        where TCollection : class, ICollection<TItem>
    {
        public MutableCollectionPool(
            IFactory<TCollection>? factory = null,
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

        protected override void OnReturn(TCollection obj)
        {
            base.OnReturn(obj);
            obj.Clear();
        }
    }
}
