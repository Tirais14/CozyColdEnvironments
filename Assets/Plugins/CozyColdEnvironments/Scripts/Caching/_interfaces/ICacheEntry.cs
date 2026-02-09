#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs.Caching
{
    public interface ICacheEntry<T>
    {
        DateTime? LastAppealTime { get; }
        TimeSpan? ExpirationTimeRelativeToNow { get; set; }
        TimeSpan IdleTime { get; }
        bool HasValue { get; }
        bool IsValid { get; }

        T? GetValue();

        bool TryGetValue([NotNullWhen(true)] out T? value);

        void SetValue(T? value);

        bool IsExpired();
    }
}
