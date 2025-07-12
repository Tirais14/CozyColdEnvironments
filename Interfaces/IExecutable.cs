namespace UTIRLib
{
    public interface IExecutable : IExecutableBase
    {
        public bool IsExecuting { get; }

        public void Execute();
    }

    public interface IExecutable<in T> : IExecutableBase
    {
        public void Execute(T arg);

        public bool IsExecuting(T arg);
    }

    //public interface IExecutable<in T1, in T2> : IExecutableBase
    //{
    //    public void Execute(T1 arg1, T2 arg2);

    //    public bool IsExecuting(T1 arg1, T2 arg2);
    //}
    //public interface IExecutable<in T1, in T2, in T3> : IExecutableBase
    //{
    //    public void Execute(T1 arg1, T2 arg2, T3 arg3);

    //    public bool IsExecuting(T1 arg1, T2 arg2, T3 arg3);
    //}
    //public interface IExecutable<in T1, in T2, in T3, in T4> : IExecutableBase
    //{
    //    public void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    //    public bool IsExecuting(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    //}
    //public interface IExecutable<in T1, in T2, in T3, in T4, in T5> : IExecutableBase
    //{
    //    public void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    //    public bool IsExecuting(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    //}
    //public interface IExecutable<in T1, in T2, in T3, in T4, in T5, in T6> : IExecutableBase
    //{
    //    public void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    //    public bool IsExecuting(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    //}
    //public interface IExecutable<in T1, in T2, in T3, in T4, in T5, in T6, in T7> : IExecutableBase
    //{
    //    public void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

    //    public bool IsExecuting(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg);
    //}
    //public interface IExecutable<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8> :
    //    IExecutableBase
    //{
    //    public void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

    //    public bool IsExecuting(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    //}
    //public interface IExecutable<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9> :
    //    IExecutableBase
    //{
    //    public void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8,
    //        T9 arg9);

    //    public bool IsExecuting(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8,
    //        T9 arg9);
    //}
}