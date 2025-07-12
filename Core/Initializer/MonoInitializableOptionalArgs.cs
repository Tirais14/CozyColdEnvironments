using UTIRLib.Attributes;

namespace UTIRLib.Initializer
{
    public abstract class MonoInitializableOptionalArgs<T> : MonoInitializable, IInitializable<T>
    {
        public void Initialize(T arg) => InitializeHelper.InitializeObject(this, arg);

        [Init]
        protected void InitializeInternal(T arg) => OnInitialize(arg);

        protected abstract void OnInitialize(T arg);
    }

    public abstract class MonoInitializableOptionalArgs<T1, T2> : MonoInitializable, IInitializable<T1, T2>
    {
        public void Initialize(T1 arg1, T2 arg2) => InitializeHelper.InitializeObject(this, arg1, arg2);

        [Init]
        protected void InitializeInternal(T1 arg1, T2 arg2) => OnInitialize(arg1, arg2);

        protected abstract void OnInitialize(T1 arg1, T2 arg2);
    }

    public abstract class MonoInitializableParams<T1, T2, T3> : MonoInitializable, IInitializable<T1, T2, T3>
    {
        public void Initialize(T1 arg1, T2 arg2, T3 arg3) => InitializeHelper.InitializeObject(this, arg1, arg2, arg3);

        [Init]
        protected void InitializeInternal(T1 arg1, T2 arg2, T3 arg3) => OnInitialize(arg1, arg2, arg3);

        protected abstract void OnInitialize(T1 arg1, T2 arg2, T3 arg3);
    }

    public abstract class MonoInitializableParams<T1, T2, T3, T4> : MonoInitializable, IInitializable<T1, T2, T3, T4>
    {
        public void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4) =>
            InitializeHelper.InitializeObject(this, arg1, arg2, arg3, arg4);

        [Init]
        protected void InitializeInternal(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => OnInitialize(arg1, arg2, arg3, arg4);

        protected abstract void OnInitialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public abstract class MonoInitializableParams<T1, T2, T3, T4, T5> : MonoInitializable, IInitializable<T1, T2, T3, T4, T5>
    {
        public void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) =>
            InitializeHelper.InitializeObject(this, arg1, arg2, arg3, arg4, arg5);

        [Init]
        protected void InitializeInternal(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => OnInitialize(arg1, arg2, arg3, arg4, arg5);

        protected abstract void OnInitialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    public abstract class MonoInitializableParams<T1, T2, T3, T4, T5, T6> : MonoInitializable, IInitializable<T1, T2, T3, T4, T5, T6>
    {
        public void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) =>
            InitializeHelper.InitializeObject(this, arg1, arg2, arg3, arg4, arg5, arg6);

        [Init]
        protected void InitializeInternal(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) =>
            OnInitialize(arg1, arg2, arg3, arg4, arg5, arg6);

        protected abstract void OnInitialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    }

    public abstract class MonoInitializableParams<T1, T2, T3, T4, T5, T6, T7> : MonoInitializable,
        IInitializable<T1, T2, T3, T4, T5, T6, T7>
    {
        public void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) =>
            InitializeHelper.InitializeObject(this, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

        [Init]
        protected void InitializeInternal(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) =>
            OnInitialize(arg1, arg2, arg3, arg4, arg5, arg6, arg7);

        protected abstract void OnInitialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    }
}