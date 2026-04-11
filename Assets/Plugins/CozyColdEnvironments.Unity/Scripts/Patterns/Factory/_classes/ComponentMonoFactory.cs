using CCEnvs.Collections;
using CCEnvs.Patterns.Factories;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Patterns.Factory
{
    public abstract class ComponentMonoFactory<T> 
        :
        MonoFactory<T>, 
        IFactory<T>,
        IFactory<Transform?, T>,
        IFactory<InstantiateParameters, T>

        where T : Component
    {
        [SerializeField]
        private T prefab;

        public override T Create() => Instantiate(prefab, transform);

        public virtual T Create(Transform? parent) => Instantiate(prefab, parent);

        public virtual T Create(InstantiateParameters prms) => Instantiate(prefab, prms);

        public object Create(params object[] args)
        {
            Guard.IsNotNull(args);

            if (args.IsEmpty())
                return Create();
            else if (args.Length == 1)
            {
                var arg = args[0];

                if (arg.Is<Transform>(out var parent))
                    return Create(parent);
                else if (arg.Is<InstantiateParameters>(out var prms))
                    return Create(prms);
            }

            throw new System.InvalidOperationException(args.Select(x => x.GetType().ToString()).JoinStringsByComma());
        }
    }

    public sealed class ComponentMonoFactory : ComponentMonoFactory<Component>
    {
    }
}
