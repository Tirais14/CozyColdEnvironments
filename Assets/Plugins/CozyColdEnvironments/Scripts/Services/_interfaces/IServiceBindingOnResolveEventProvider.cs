#nullable enable
namespace CCEnvs.Services
{
    public interface IServiceBindingOnResolveEventProvider
    {
        OnResolvedAction? OnResolve { get; }
    }

    public interface IServiceBindingOnResolveEventProvider<T> : IServiceBindingOnResolveEventProvider
    {
        new OnResolvedAction<T>? OnResolve { get; }

        OnResolvedAction? IServiceBindingOnResolveEventProvider.OnResolve => OnResolve;
    }
}
