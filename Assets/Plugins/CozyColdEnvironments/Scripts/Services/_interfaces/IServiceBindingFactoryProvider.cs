using CCEnvs.Patterns.Factories;

#nullable enable
namespace CCEnvs.Services
{
    public interface IServiceBindingFactoryProvider
    {
        IFactory Factory { get; }
    }

    public interface IServiceBindingFactoryProvider<T> : IServiceBindingFactoryProvider
    {
        new IFactory<T> Factory { get; }

        IFactory IServiceBindingFactoryProvider.Factory => Factory;
    }
}
