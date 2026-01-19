#pragma warning disable S2436
namespace CCEnvs.Patterns.Factories
{
#nullable enable

    public interface IFactory
    {
        public object Create(params object[] args);
    }

    public interface IFactory<out TOut> : IFactory
    {

        public TOut Create();

        object IFactory.Create(params object[] args) => Create()!;
    }

    public interface IFactory<in T, out TOut> : IFactory
    {
        public TOut Create(T arg);

        object IFactory.Create(params object[] args) => Create((T)args[0])!;
    }

    public interface IFactory<in T, in T1, out TOut> : IFactory
    {
        public TOut Create(T arg, T1 arg1);

        object IFactory.Create(params object[] args)
        {
            return Create((T)args[0], (T1)args[1])!;
        }
    }

    public interface IFactory<in T, in T1, in T2, out TOut> : IFactory
    {
        public TOut Create(T arg, T1 arg1, T2 arg2);

        object IFactory.Create(params object[] args)
        {
            return Create((T)args[0], (T1)args[1], (T2)args[2])!;
        }
    }
}