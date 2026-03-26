using CCEnvs.Attributes;
using CCEnvs.Patterns.Factories;
using CCEnvs.Pools;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Caching;
using CCEnvs.Threading.Tasks;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Components;
using UnityEngine;
using CCEnvs.Threading.Tasks;

#nullable enable
namespace CCEnvs.Unity
{
    public class ObjectPoolView<T> : CCBehaviour
        where T : class
    {
        [SerializeField, Min(0f)]
        protected int initialCount;

        [SerializeField, Min(1f)]
        protected int preheatBatchSize = 3;

        [SerializeField, OptionalField]
        protected GameObject prefab;

        public ObjectPool<T> Pool { get; private set; } = null!;

        public Transform InstantiatedParent { get; private set; } = null!;

        protected override void Start()
        {
            base.Start();

            var factory = CreateFactory();
            Pool = new ObjectPool<T>(factory);
            TryPreheatPool();
        }

        private IFactory<T> CreateFactory()
        {
            var instanceParentName = TypeofCache<T>.Type.Name.Split('.')[^1] + 's';

            if (!GameObjectQuery.Scene.WithName(instanceParentName, byFullName: true)
                .Transform()
                .Lax()
                .TryGetValue(out var instantiatedParent))
            {
                instantiatedParent = new GameObject(instanceParentName).transform;
            }

            InstantiatedParent = instantiatedParent;

            return Factory.Create(this,
                @this =>
                {
                    var instance = Instantiate(@this.prefab, InstantiatedParent);

                    if (TypeCache<T>.IsUnityGameObject)
                        return instance.CastTo<T>();

                    return instance.Q().Component<T>().Strict();
                });
        }

        private void TryPreheatPool()
        {
            if (initialCount <= 0)
                return;

            var op = new ObjectPoolPreheatOperation<T>(
                Pool,
                initialCount,
                batchSize: preheatBatchSize
                );

            op.ExecuteAsync(destroyCancellationToken).ForgetByPrintException(this);
        }
    }
}
