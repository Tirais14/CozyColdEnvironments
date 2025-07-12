using System;

#nullable enable

namespace UTIRLib.Initializer
{
    public abstract class MonoInitializable : MonoX, IInitializable
    {
        public bool IsInitialized { get; protected set; }

        public void Initialize() => InitializeHelper.InitializeObject<Action>(this, InitializeInternal,
            InitializeParameters.ArgumentsMayBeNull);

        protected abstract void InitializeInternal();
    }

    public abstract class MonoInitializable<T> : MonoX, IInitializable<T>
    {
        public bool IsInitialized { get; protected set; }

        public void Initialize(T arg) => InitializeHelper.InitializeObject<Action<T>>(this, InitializeInternal,
            InitializeParameters.ArgumentsMayBeNull, arg);

        protected abstract void InitializeInternal(T arg);
    }

    public abstract class MonoInitializable<T1, T2> : MonoX, IInitializable<T1, T2>
    {
        public bool IsInitialized { get; protected set; }

        public void Initialize(T1 arg1, T2 arg2) =>
            InitializeHelper.InitializeObject<Action<T1, T2>>(this, InitializeInternal,
                InitializeParameters.ArgumentsMayBeNull, arg1, arg2);

        protected abstract void InitializeInternal(T1 arg1, T2 arg2);
    }

    public abstract class MonoInitializable<T1, T2, T3> : MonoX, IInitializable<T1, T2, T3>
    {
        public bool IsInitialized { get; protected set; }

        public void Initialize(T1 arg1, T2 arg2, T3 arg3) =>
            InitializeHelper.InitializeObject<Action<T1, T2, T3>>(this, InitializeInternal,
                InitializeParameters.ArgumentsMayBeNull, arg1, arg2, arg3);

        protected abstract void InitializeInternal(T1 arg1, T2 arg2, T3 arg3);
    }

    public abstract class MonoInitializable<T1, T2, T3, T4> : MonoX, IInitializable<T1, T2, T3, T4>
    {
        public bool IsInitialized { get; protected set; }

        public void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4) =>
            InitializeHelper.InitializeObject<Action<T1, T2, T3, T4>>(this, InitializeInternal,
                InitializeParameters.ArgumentsMayBeNull, arg1, arg2, arg3, arg4);

        protected abstract void InitializeInternal(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public abstract class MonoInitializable<T1, T2, T3, T4, T5> : MonoX, IInitializable<T1, T2, T3, T4, T5>
    {
        public bool IsInitialized { get; protected set; }

        public void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) =>
            InitializeHelper.InitializeObject<Action<T1, T2, T3, T4, T5>>(this, InitializeInternal,
                InitializeParameters.ArgumentsMayBeNull, arg1, arg2, arg3, arg4, arg5);

        protected abstract void InitializeInternal(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    public abstract class MonoInitializable<T1, T2, T3, T4, T5, T6> : MonoX, IInitializable<T1, T2, T3, T4, T5, T6>
    {
        public bool IsInitialized { get; protected set; }

        public void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) =>
            InitializeHelper.InitializeObject<Action<T1, T2, T3, T4, T5, T6>>(this, InitializeInternal,
                InitializeParameters.ArgumentsMayBeNull, arg1, arg2, arg3, arg4, arg5, arg6);

        protected abstract void InitializeInternal(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    }

    public abstract class MonoInitializable<T1, T2, T3, T4, T5, T6, T7> : MonoX,
        IInitializable<T1, T2, T3, T4, T5, T6, T7>
    {
        public bool IsInitialized { get; protected set; }

        public void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) =>
            InitializeHelper.InitializeObject<Action<T1, T2, T3, T4, T5, T6, T7>>(this, InitializeInternal,
                InitializeParameters.ArgumentsMayBeNull, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

        protected abstract void InitializeInternal(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    }
}