using Cysharp.Threading.Tasks;

#nullable enable
namespace UTIRLib.Initables
{
    public abstract class MonoXInitableAsync : MonoX, IInitableAsync
    {
        public bool IsInited { get; private set; }

        protected abstract UniTask OnInit();

        async UniTask IInitableAsync.InitAsync()
        {
            await OnInit();
        }
    }
}
