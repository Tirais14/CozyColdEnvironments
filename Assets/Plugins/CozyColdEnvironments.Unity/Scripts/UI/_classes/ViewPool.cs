using CCEnvs.Patterns.Factories;
using CCEnvs.UnityX.Pools;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI
{
    public class ViewPool<TView> : ComponentPool<TView>
        where TView : Component, IView
    {
        public ViewPool(
            IFactory<TView>? factory = null,
            int capacity = 4,
            int? maxSize = null
            )
            :
            base(factory, capacity, maxSize)
        {
        }

        protected override void OnReturn(TView obj)
        {
            base.OnReturn(obj);
            obj.TrySetModel(null);
        }
    }
}
