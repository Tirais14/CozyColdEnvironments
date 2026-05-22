#nullable enable
namespace CCEnvs
{
    public interface IManagedConvertible
    {
        object ConvertToManaged();
    }

    public interface IManagedConvertible<T> : IManagedConvertible
    {
        new T ConvertToManaged();

        TConcrete ConvertToManagedT<TConcrete>() where TConcrete : T
        {
            return (TConcrete)ConvertToManaged()!;
        }

        object IManagedConvertible.ConvertToManaged() => ConvertToManaged()!;
    }
}
