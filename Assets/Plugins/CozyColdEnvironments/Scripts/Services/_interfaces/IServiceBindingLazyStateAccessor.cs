#nullable enable
namespace CCEnvs.Services
{
    public interface IServiceBindingLazyStateAccessor : IServiceBindingLazyStateProvider
    {
        new bool IsLazy { get; set; }

        bool IServiceBindingLazyStateProvider.IsLazy => IsLazy;
    }
}
