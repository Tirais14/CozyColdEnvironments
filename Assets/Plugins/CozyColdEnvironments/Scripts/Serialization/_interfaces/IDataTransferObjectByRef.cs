#nullable enable
namespace CCEnvs.Serialization
{
    public interface IDataTransferObjectByRef<T> : IDataTransferObject<T>
    {
        ref T MaterializeRef();

        T IDataTransferObject<T>.Materialize()
        {
            return MaterializeRef();
        }
    }
}
