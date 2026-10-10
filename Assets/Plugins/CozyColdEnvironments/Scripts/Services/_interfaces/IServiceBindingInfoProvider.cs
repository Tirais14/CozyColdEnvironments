using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Services
{
    public interface IServiceBindingInfoProvider
    {
        object Target { get; }
        object? ID { get; }

        IEnumerable<Type> Contracts { get; }

        bool IgnoreBinded { get; }
        bool IsFactory { get; }
    }
}
