using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Services
{
    public interface IServiceBinderInterfacesProvider
    {
        IReadOnlyList<Type> Interfaces { get; }
    }
}
