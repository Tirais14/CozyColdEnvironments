#nullable enable
using System;

namespace CCEnvs.Properties
{
    public interface ILazyProperty<T>
    {
        T Value { get; }
        bool ValueCreated { get; }
        bool RecreateValueIfDefault { get; }
        bool HasValue { get; }
    }
}
