using Cysharp.Threading.Tasks;

#nullable enable
namespace UTIRLib.Initables
{
    public interface IInitableAsync : IInitableBase
    {
        UniTask Init();
    }
}
