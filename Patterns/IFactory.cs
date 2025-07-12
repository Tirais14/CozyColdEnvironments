namespace UTIRLib.Patterns.Factory
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

    public interface IFactory<in T1, in T2, out TOut>
    {
        public TOut Create(T1 arg1, T2 arg2);
    }

    public interface IFactory<in T1, in T2, in T3, out TOut>
    {
        public TOut Create(T1 arg1, T2 arg2, T3 arg3);
    }
}