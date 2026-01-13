#nullable enable
using System;

namespace CCEnvs.Caching
{
    public interface IReferenceCacheEntry<T> where T : class
    {
        DateTime? LastAppealTime { get; }
        TimeSpan ExpirationTimeRelativeToNow { get; set; }
        TimeSpan IdleTime { get; }

        T? GetValue();

        void SetValue(T? value);

        bool IsExpired();
    }
}
