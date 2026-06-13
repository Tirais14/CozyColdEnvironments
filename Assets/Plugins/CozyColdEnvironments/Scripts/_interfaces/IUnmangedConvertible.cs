#nullable enable
namespace CCEnvs
{
    public interface IUnmangedConvertible<T>
        where T : unmanaged
    {
        T ToUnmanaged();
    }
}
