using Cysharp.Threading.Tasks;

#nullable enable
namespace CozyColdEnvironments.Initables
{
    public abstract class MonoXInitableAsync : MonoCC, IInitableAsync
    {
        public bool IsInited { get; private set; }

        protected abstract UniTask OnInitAsync();

        async UniTask IInitableAsync.InitAsync()
        {
            await OnInitAsync();
        }
    }
}
