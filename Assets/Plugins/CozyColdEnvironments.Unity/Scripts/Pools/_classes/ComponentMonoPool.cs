using CCEnvs.Unity.Patterns.Factory;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Pools
{
    public abstract class ComponentMonoPool<TComponent, TFactory> 
        :
        MonoObjectPool<TComponent, ComponentPool<TComponent>, TFactory>

        where TComponent : Component
        where TFactory : MonoFactory<TComponent>
    {
        protected override ComponentPool<TComponent> CreatePool()
        {
            return new ComponentPool<TComponent>(factory, capacity: capacity);
        }
    }

    public abstract class ComponentMonoPool<TComponent> : ComponentMonoPool<TComponent, ComponentMonoFactory<TComponent>>
        where TComponent : Component
    {

    }

    public class ComponentMonoPool : ComponentMonoPool<Component>
    {
    }
}
