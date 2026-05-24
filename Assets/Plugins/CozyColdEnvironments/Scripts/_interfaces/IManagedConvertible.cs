#nullable enable
namespace CCEnvs
{
    public interface IManagedConvertible
    {
        object ConvertToManaged();
    }

    public interface IManagedConvertible<T> : IManagedConvertible
    {
        new T ToManaged();

        TConcrete ToManagedT<TConcrete>() where TConcrete : T
        {
            return (TConcrete)ToManaged()!;
        }

        object IManagedConvertible.ConvertToManaged() => ToManaged()!;
    }
}
