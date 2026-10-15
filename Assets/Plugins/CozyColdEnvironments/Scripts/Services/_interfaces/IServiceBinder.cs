using System;

#nullable enable
namespace CCEnvs.Services
{
    public interface IServiceBinder
    {
        Type Contract { get; }

        object Instance { get; }
    }
}
