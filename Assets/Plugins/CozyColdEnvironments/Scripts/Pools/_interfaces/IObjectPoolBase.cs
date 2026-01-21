using R3;
using System;

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
    }

    public interface IObjectPoolBase<T> : IObjectPoolBase
        where T : class
    {
        void Return(T? obj);

        Observable<T> ObserveReturn();

        Observable<T> ObserveGet();
    }
}
