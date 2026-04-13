using CCEnvs.Collections;
using CCEnvs.Patterns.Factories;
using CCEnvs.Unity.Components;
using CommunityToolkit.Diagnostics;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Patterns.Factory
{
    public abstract class MonoFactory : CCBehaviour, IFactory
    {
        public abstract object Create(params object[] args);
    }

    public abstract class MonoFactory<T> : MonoFactory, IFactory<T>
    {
        public abstract T Create();

        public override object Create(params object[] args) => Create()!;
    }

    public abstract class MonoFactory<T, TArg> : MonoFactory, IFactory<TArg, T>
    {
        public abstract T Create(TArg arg);

        public override object Create(params object[] args)
        {
            Guard.IsNotNull(args);

            if (args.IsEmpty() || args[0] is not TArg arg)
                throw new System.InvalidOperationException($"Args must be: [0] - {typeof(TArg)}");

            return Create(arg)!;
        }
    }
}
