using CCEnvs.Patterns.Factories;
using Cysharp.Threading.Tasks;

#nullable enable
namespace CCEnvs.Unity
{
    public abstract class MonoFactoryAsync<T> : IFactoryAsync<T>
    {
        public abstract
#if UNITASK_PLUGIN
            UniTask<T>
#else
            System.Threading.ValueTask<T>
#endif
            Create();
    }
}
