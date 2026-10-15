#nullable enable
namespace CCEnvs.Services
{
    public interface IServiceBinderIfNotBoundProvider
    {
        bool IgnoreIfBound { get; }
    }
}
