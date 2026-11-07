#nullable enable
namespace CCEnvs.Services
{
    public interface IServiceBindingNotBoundStateAccessor : IServiceBindingNotBoundStateProvider
    {
        new bool IgnoreIfBound { get; set; }

        bool IServiceBindingNotBoundStateProvider.IgnoreIfBound => IgnoreIfBound;
    }
}
