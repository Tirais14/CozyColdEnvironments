using UTIRLib.Init;

#nullable enable
namespace UTIRLib
{
    public abstract class MonoXInitable : MonoX, IInitable
    {
        public bool IsInited { get; private set; }

        /// <summary>
        /// Realize this method, to manually Init. Called in Awake
        /// </summary>
        protected abstract void OnInit();

        void IInitable.Init() => IsInited = true;
    }
}
