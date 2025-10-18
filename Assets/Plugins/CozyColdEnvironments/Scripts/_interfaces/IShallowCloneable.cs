#nullable enable
namespace CCEnvs
{
    public interface IShallowCloneable
    {
        object ShallowClone();
    }
    public interface IShallowCloneable<out T> : IShallowCloneable
    {
        new T ShallowClone();

        object IShallowCloneable.ShallowClone() => ShallowClone()!;
    }
}
