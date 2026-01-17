#pragma warning disable S2436
namespace CCEnvs.Patterns.Factories
{
#nullable enable

    public interface IFactory
    {
        public object Create(params object[] args);
    }

    public interface IFactory<out T>
    {

        public T Create();
    }

    public interface IFactory<in T1, out TOut>
    {
        public TOut Create(T1 arg);
    }

    public interface IFactory<in T, in T1, out TOut>
    {
        public TOut Create(T arg, T1 arg1);
    }

    public interface IFactory<in T, in T1, in T2, out TOut>
    {
        public TOut Create(T arg, T1 arg1, T2 arg2);
    }
}