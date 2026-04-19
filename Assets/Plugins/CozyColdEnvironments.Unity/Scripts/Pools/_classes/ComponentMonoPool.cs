using CCEnvs.Patterns.Factories;
using CCEnvs.Unity.Patterns.Factory;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Pools
{
    public abstract class ComponentMonoPool<TComponent, TFactory> 
        :
        MonoObjectPool<TComponent, ComponentPool<TComponent>, TFactory>

        where TComponent : Component
        where TFactory : Component, IFactory<TComponent>
    {
        public override void Return(TComponent? obj)
        {
            base.Return(obj);

            if (obj.IsNotNull())
                obj.transform.SetParent(cTransform);
        }

        protected override ComponentPool<TComponent> CreatePool()
        {
            return new ComponentPool<TComponent>(factory, capacity: capacity);
        }
    }

    public abstract class ComponentMonoPool<TComponent> : ComponentMonoPool<TComponent, UnityObjectMonoFactory<TComponent>>
        where TComponent : Component
    {

    }

    public class ComponentMonoPool : ComponentMonoPool<Component>
    {
    }
}
