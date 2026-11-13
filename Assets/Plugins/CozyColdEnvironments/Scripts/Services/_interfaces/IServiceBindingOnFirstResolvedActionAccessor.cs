#nullable enable
namespace CCEnvs.Services
{
    public interface IServiceBindingOnFirstResolvedActionAccessor
    {
        OnResolvedAction? EvOnFirstResolved { get; set; }

        //OnResolveAction? IServiceBindingOnResolveEventProvider.OnResolve => OnReoslve;
    }

    public interface IServiceBindingOnFirstResolvedActionAccessor<T> : IServiceBindingOnFirstResolvedActionAccessor
    {
        new OnResolvedAction<T>? EvOnFirstResolved { get; set; }

        //OnResolveAction<T>? IServiceBindingOnResolveEventProvider<T>.OnResolve => OnResolve;
        OnResolvedAction? IServiceBindingOnFirstResolvedActionAccessor.EvOnFirstResolved {
            get => EvOnFirstResolved;
            set => EvOnFirstResolved = (OnResolvedAction<T>?)value;
        }
    }
}
