using CCEnvs.Patterns.Factories;
using CCEnvs.Threading;
using CCEnvs.Unity.Components;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Patterns.Factory
{
    public class UnityObjectMonoFactoryAsync<TOut>
        :
        CCBehaviour,
        IFactory<CancellationToken, ValueTask<TOut>>,
        IFactory<Transform?, CancellationToken, ValueTask<TOut>>,
        IFactory<InstantiateParameters, CancellationToken, ValueTask<TOut>>,
        IFactory<int, CancellationToken, ValueTask<TOut[]>>,
        IFactory<int, Transform?, CancellationToken, ValueTask<TOut[]>>,
        IFactory<int, InstantiateParameters, CancellationToken, ValueTask<TOut[]>>

        where TOut : UnityEngine.Object
    {
        [SerializeField]
        protected TOut prefab;

#if ZENJECT_PLUGIN
        protected Zenject.DiContainer? diContainer;
#endif

        protected UnityObjectFactoryAsync<TOut> core { get; private set; } = null!;

#if ZENJECT_PLUGIN
        [Zenject.Inject]
        private void Construct(Zenject.DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }
#endif

        protected override void Start()
        {
            base.Start();
#if ZENJECT_PLUGIN
            core = new UnityObjectFactoryAsync<TOut>(prefab, diContainer, cTransform);
#else
            core = new UnityObjectFactoryAsync<TOut>(prefab, cTransform);
#endif
        }

        public ValueTask<TOut> Create(CancellationToken cancellationToken)
        {
            using var _ = destroyCancellationToken.TryLinkTokens(out cancellationToken);
            return core.Create(cancellationToken);
        }
        public ValueTask<TOut> Create(Transform? parent, CancellationToken cancellationToken)
        {
            using var _ = destroyCancellationToken.TryLinkTokens(out cancellationToken);
            return core.Create(parent, cancellationToken);
        }
        public ValueTask<TOut> Create(InstantiateParameters prms, CancellationToken cancellationToken)
        {
            using var _ = destroyCancellationToken.TryLinkTokens(out cancellationToken);
            return core.Create(prms, cancellationToken);
        }
        public ValueTask<TOut[]> Create(int count, CancellationToken cancellationToken)
        {
            using var _ = destroyCancellationToken.TryLinkTokens(out cancellationToken);
            return core.Create(count, cancellationToken);
        }
        public ValueTask<TOut[]> Create(int count, Transform? parent, CancellationToken cancellationToken)
        {
            using var _ = destroyCancellationToken.TryLinkTokens(out cancellationToken);
            return core.Create(count, parent, cancellationToken);
        }
        public ValueTask<TOut[]> Create(int count, InstantiateParameters prms, CancellationToken cancellationToken)
        {
            using var _ = destroyCancellationToken.TryLinkTokens(out cancellationToken);
            return core.Create(count, prms, cancellationToken);
        }
    }
}
