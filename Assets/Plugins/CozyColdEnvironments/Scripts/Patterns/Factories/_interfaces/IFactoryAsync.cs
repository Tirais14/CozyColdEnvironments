using UnityEngine;

#nullable enable
#pragma warning disable S2436
namespace CCEnvs.Patterns.Factory
{
    public interface IFactoryAsync
    {
#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask<object>
#else
            System.Threading.Tasks.ValueTask<object>
#endif
            Create(params object[] args);
    }

    public interface IFactoryAsync<TOut>
    {
#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask<TOut>
#else
            System.Threading.Tasks.ValueTask<TOut>
#endif
            Create();
    }

    public interface IFactoryAsync<in T, TOut>
    {
#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask<TOut>
#else
            System.Threading.Tasks.ValueTask<TOut>
#endif
            Create(T arg);
    }

    public interface IFactoryAsync<in T, in T1, TOut>
    {
#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask<TOut>
#else
            System.Threading.Tasks.ValueTask<TOut>
#endif
            Create(T arg, T1 arg1);
    }

    public interface IFactoryAsync<in T, in T1, in T2, TOut>
    {
#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask<TOut>
#else
            System.Threading.Tasks.ValueTask<TOut>
#endif
            Create(T arg, T1 arg1, T2 arg2);
    }
}
