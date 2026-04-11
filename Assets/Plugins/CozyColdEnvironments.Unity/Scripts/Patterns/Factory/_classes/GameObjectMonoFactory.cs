using CCEnvs.Collections;
using CCEnvs.Patterns.Factories;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Patterns.Factory
{
    public sealed class GameObjectMonoFactory
        : 
        MonoFactory<GameObject>,
        IFactory<GameObject>,
        IFactory<Transform?, GameObject>,
        IFactory<InstantiateParameters, GameObject>
    {
        [SerializeField]
        private GameObject prefab;

        public override GameObject Create() => Instantiate(prefab, transform);

        public GameObject Create(Transform? parent) => Instantiate(prefab, parent);

        public GameObject Create(InstantiateParameters prms) => Instantiate(prefab, prms);

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
}
