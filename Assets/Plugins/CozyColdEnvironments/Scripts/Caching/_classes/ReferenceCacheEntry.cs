using System;

#nullable enable
#pragma warning disable S6561
namespace CCEnvs.Caching
{
    public class ReferenceCacheEntry<T> : IReferenceCacheEntry<T>
        where T : class
    {
        private readonly WeakReference<T?> valueRef;

        public DateTime? LastAppealTime { get; private set; }
        public TimeSpan ExpirationTimeRelativeToNow { get; set; }
        public TimeSpan IdleTime {
            get
            {
                if (LastAppealTime is null)
                    return TimeSpan.Zero;

                return DateTime.Now - LastAppealTime.Value;
            }
        }

        public ReferenceCacheEntry(T? value)
        {
            valueRef = new WeakReference<T?>(value);
        }

        public T? GetValue()
        {
            valueRef.TryGetTarget(out var value);

            LastAppealTime = DateTime.Now;

            return value;
        }

        public void SetValue(T? value)
        {
            valueRef.SetTarget(value);
            LastAppealTime = DateTime.Now;
        }

        public bool IsExpired()
        {
            return !valueRef.TryGetTarget(out _)
                   ||
                   IdleTime > ExpirationTimeRelativeToNow;
        }
    }
}
