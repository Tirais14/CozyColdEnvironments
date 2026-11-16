#nullable enable
namespace CCEnvs
{
    public interface IIDProvider
    {
        object? ID { get; }
    }
    public interface IIDProvider<out T> : IIDProvider
    {
        new T ID { get; }

        object IIDProvider.ID => ID!;
    }
}
