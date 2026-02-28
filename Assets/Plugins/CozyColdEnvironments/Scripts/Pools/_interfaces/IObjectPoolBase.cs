using R3;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Pools
{
    public interface IObjectPoolBase : IDisposable
    {
        int ActiveCount { get; }
        int InactiveCount { get; }
        int Count { get; }
        bool HasFactory { get; }

        void Clear();

        bool IsActiveObject(object obj);
    }

    public interface IObjectPoolBase<T> : IObjectPoolBase
        where T : class
    {
        void Return(T? obj);

        bool IsActiveObject(T obj);

        Observable<T> ObserveReturn();

        Observable<T> ObserveGet();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IObjectPoolBase.IsActiveObject(object obj)
        {
            return IsActiveObject((T)obj);
        }
    }
}
