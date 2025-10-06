#nullable enable
using CCEnvs.Unity.Components;

namespace CCEnvs.Unity.Initables
{
    public abstract class MonoCCInitable : CCBehaviour, IInitable
    {
        public bool IsInited { get; private set; }

        /// <summary>
        /// Realize this method, to manually Init. Called in Awake
        /// </summary>
        protected abstract void OnInit();

        void IInitable.Init()
        {
            OnInit();
            IsInited = true;
        }
    }
}
