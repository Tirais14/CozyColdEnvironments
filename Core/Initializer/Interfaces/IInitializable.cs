#nullable enable

namespace UTIRLib.Initializer
{
    public interface IInitializable : IInitializableBase
    {
        public void Initialize();
    }

    public interface IInitializable<in T> : IInitializableBase
    {
        public void Initialize(T arg);
    }

    public interface IInitializable<in T1, in T2> : IInitializableBase
    {
        public void Initialize(T1 arg1, T2 arg2);
    }

    public interface IInitializable<in T1, in T2, in T3> : IInitializableBase
    {
        public void Initialize(T1 arg1, T2 arg2, T3 arg3);
    }

    public interface IInitializable<in T1, in T2, in T3, in T4> : IInitializableBase
    {
        public void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public interface IInitializable<in T1, in T2, in T3, in T4, in T5> : IInitializableBase
    {
        public void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    public interface IInitializable<in T1, in T2, in T3, in T4, in T5, in T6> : IInitializableBase
    {
        public void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    }

    public interface IInitializable<in T1, in T2, in T3, in T4, in T5, in T6, in T7> : IInitializableBase
    {
        public void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    }

    public interface IInitializable<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8> : IInitializableBase
    {
        public void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    }

    public interface IInitializable<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9> : IInitializableBase
    {
        public void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
    }
}