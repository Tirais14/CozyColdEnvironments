#nullable enable
namespace CCEnvs
{
    public interface IObjectProvider
    {
        object InternalObject { get; }
    }
    public interface IObjectProvider<out T> : IObjectProvider
    {
        new T InternalObject { get; }

        object IObjectProvider.InternalObject => InternalObject!;
    }
}
