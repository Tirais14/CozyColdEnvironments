using Cysharp.Threading.Tasks;

#nullable enable
namespace CCEnvs.Unity.Initables
{
    public interface IInitableAsync : IInitableBase
    {
        UniTask InitAsync();
    }
}
