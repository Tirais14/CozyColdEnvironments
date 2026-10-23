#nullable enable
namespace CCEnvs
{
    public interface IFactoryNonGeneric
    {
        object Create(params object[] args);
    }
}
