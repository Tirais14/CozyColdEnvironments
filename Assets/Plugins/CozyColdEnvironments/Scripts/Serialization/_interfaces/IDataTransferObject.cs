#nullable enable
namespace CCEnvs.Serialization
{
    public interface IDataTransferObject
    {
        object Materialize();
    }

    public interface IDataTransferObject<T> : IDataTransferObject
    {
        new T Materialize();

        object IDataTransferObject.Materialize() => Materialize()!;
    }
}
