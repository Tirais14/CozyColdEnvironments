using System;

#nullable enable
namespace CCEnvs.Services
{
    public interface IServiceBinder
    {
        Type Contract { get; }
    }

    public interface IServiceBinder<T> : IServiceBinder
    {
        Type IServiceBinder.Contract => typeof(T);
    }
}
