using Cysharp.Threading.Tasks;

#nullable enable
namespace CCEnvs.Unity.Initables
{
    public abstract class MonoCCInitableAsync : MonoCC, IInitableAsync
    {
        public bool IsInited { get; private set; }

        protected abstract UniTask OnInitAsync();

        async UniTask IInitableAsync.InitAsync()
        {
            await OnInitAsync();
        }
    }
}
