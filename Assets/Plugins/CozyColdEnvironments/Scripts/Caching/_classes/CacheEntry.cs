using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
#pragma warning disable S6561
namespace CCEnvs.Caching
{
    public class CacheEntry<T> : ICacheEntry<T>
    {
        private T? value;

        public DateTime? LastAppealTime { get; private set; }
        public TimeSpan? ExpirationTimeRelativeToNow { get; set; }

        public TimeSpan IdleTime {
            get
            {
                if (LastAppealTime is null)
                    return TimeSpan.Zero;

                return DateTime.Now - LastAppealTime.Value;
            }
        }

        public bool HasValue => value.IsNotNull();
        public bool IsValid => HasValue && !IsExpired();

        public CacheEntry(T? value)
        {
            this.value = value;
        }

        public T? GetValue()
        {
            LastAppealTime = DateTime.Now;

            return value;
        }

        public bool TryGetValue([NotNullWhen(true)] out T? value)
        {
            if (this.value.IsNull())
            {
                value = default;
                return false;
            }

            value = this.value; 
            return true;
        }

        public void SetValue(T? value)
        {
            this.value = value;
            LastAppealTime = DateTime.Now;
        }

        public bool IsExpired()
        {
            if (ExpirationTimeRelativeToNow is null)
                return false;

            return IdleTime > ExpirationTimeRelativeToNow.Value;
        }
    }
}
