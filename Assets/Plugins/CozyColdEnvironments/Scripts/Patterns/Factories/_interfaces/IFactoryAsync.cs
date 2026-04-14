#nullable enable
#pragma warning disable S2436
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Factories
{
    public interface IFactoryAsync
    {
        ValueTask<object> Create(params object[] args);
    }

    public interface IFactoryAsync<TOut>
    {
        ValueTask<TOut> Create();
    }

    public interface IFactoryAsync<in T, TOut>
    {
        ValueTask<TOut> Create(T arg);
    }

    public interface IFactoryAsync<in T, in T1, TOut>
    {
        ValueTask<TOut> Create(T arg, T1 arg1);
    }

    public interface IFactoryAsync<in T, in T1, in T2, TOut>
    {
        ValueTask<TOut> Create(T arg, T1 arg1, T2 arg2);
    }
}
