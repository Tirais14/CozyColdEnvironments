using CCEnvs.Reflection;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Disposables
{
    public static class CCDisposable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DisposableLight<TState> CreateLight<TState>(
            TState state,
            Action<TState> disposeAction
            )
        {
            return new DisposableLight<TState>(state, disposeAction);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DisposableLight CreateLight(Action disposeAction)
        {
            return new DisposableLight(disposeAction);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dispose<T>(ref T? disposable)
            where T : IDisposable
        {
            if (disposable.IsNull())
                return;

            disposable.Dispose();
            disposable = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dispose<T>(ref DisposableLight<T> disposable)
        {
            disposable.Dispose();
            disposable = default;
        }
    }
}
