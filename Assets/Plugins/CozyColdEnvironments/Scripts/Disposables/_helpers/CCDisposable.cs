using System;
using System.Runtime.CompilerServices;
using CCEnvs.Reflection;

#nullable enable
namespace CCEnvs.Disposables
{
    public static class CCDisposable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LightDisposable<TState> CreateLight<TState>(
            TState state,
            Action<TState> disposeAction
            )
        {
            return new LightDisposable<TState>(state, disposeAction);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LightDisposable CreateLight(Action disposeAction)
        {
            return new LightDisposable(disposeAction);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfDisposed<T>(bool disposed)
        {
            if (disposed)
                throw new ObjectDisposedException(TypeofCache<T>.Type.Name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfDisposed<T>(this T _, bool disposed)
        {
            if (disposed)
                throw new ObjectDisposedException(TypeofCache<T>.Type.Name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfDisposed<T>(int disposed)
        {
            if (disposed >= 1)
                throw new ObjectDisposedException(TypeofCache<T>.Type.Name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfDisposed<T>(this T _, int disposed)
        {
            if (disposed >= 1)
                throw new ObjectDisposedException(TypeofCache<T>.Type.Name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDisposed(int disposed)
        {
            if (disposed <= 0)
                return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDisposed(bool disposed)
        {
            return disposed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dispose(ref IDisposable? disposable)
        {
            if (disposable.IsNull())
                return;

            disposable.Dispose();
            disposable = null;
        }
    }
}
