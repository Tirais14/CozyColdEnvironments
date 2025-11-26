using System;

#nullable enable
namespace CCEnvs
{
    public readonly struct CachingOptions
    {
        public TimeSpan LifeTime { get; }

        public CachingOptions(TimeSpan lifeTime)
        {
            LifeTime = lifeTime;
        }
    }
}
