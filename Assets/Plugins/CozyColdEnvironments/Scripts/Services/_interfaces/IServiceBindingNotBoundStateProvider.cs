#nullable enable
namespace CCEnvs.Services
{
    public interface IServiceBindingNotBoundStateProvider
    {
        bool IgnoreIfBound { get; }
    }
}
