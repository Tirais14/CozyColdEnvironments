#nullable enable
namespace CCEnvs.Services
{
    public interface IServiceBindingInstanceProvider
    {
        object Instance { get; }
    }

    public interface IServiceBindingInstanceProvider<T> : IServiceBindingInstanceProvider
    {
        new T Instance { get; }

        object IServiceBindingInstanceProvider.Instance => Instance!;
    }
}
