using Cysharp.Threading.Tasks;

#nullable enable
namespace CozyColdEnvironments.Initables
{
    public interface IInitableAsync : IInitableBase
    {
        UniTask InitAsync();
    }
}
