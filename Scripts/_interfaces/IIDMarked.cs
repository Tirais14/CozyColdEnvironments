#nullable enable
namespace CCEnvs
{
    public interface IIDMarked
    {
        object ID { get; }
    }
    public interface IIDMarked<out T> : IIDMarked
    {
        new T ID { get; }

        object IIDMarked.ID => ID!;
    }
}
