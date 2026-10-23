#nullable enable
namespace CCEnvs.Services
{
    public interface IServiceBindingLazyStateProvider
    {
        bool IsLazy { get; }
    }
}
