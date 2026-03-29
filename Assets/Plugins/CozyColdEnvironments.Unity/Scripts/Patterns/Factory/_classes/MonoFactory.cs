using CCEnvs.Patterns.Factories;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Patterns.Factory
{
    public abstract class MonoFactory : MonoBehaviour, IFactory
    {
        public abstract object Create(params object[] args);
    }

    public abstract class MonoFactory<T> : MonoBehaviour, IFactory<T>
    {
        public abstract T Create();
    }

    public abstract class MonoFactory<T, TArg> : MonoBehaviour, IFactory<TArg, T>
    {
        public abstract T Create(TArg arg);
    }
}
