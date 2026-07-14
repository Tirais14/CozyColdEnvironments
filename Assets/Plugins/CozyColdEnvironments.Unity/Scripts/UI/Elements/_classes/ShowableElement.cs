#nullable enable
using CCEnvs.UnityX.Injections;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine;

namespace CCEnvs.UnityX.UI.Elements
{
    [RequireComponent(typeof(PanelRenderer))]
    public class ShowableElement 
        :
        ShowableBase<IShowableElement>,
        IShowableElement
    {
        private bool isUIReloadBinded;

        [field: GetBySelf]
        public PanelRenderer renderer { get; private set; } = null!;

        public VisualElement? rendererRoot { get; private set; }

        protected override void Start()
        {
            base.Start();
            InitAsync().Forget(ex => this.PrintException(ex));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            renderer.RegisterUIReloadCallback(OnUIReload);
            isUIReloadBinded = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            renderer.UnregisterUIReloadCallback(OnUIReload);
            isUIReloadBinded = false;
        }

        public override void Redraw()
        {
            if (!isUIReloadBinded)
                return;

            HideCore();
            renderer.UnregisterUIReloadCallback(OnUIReload);
            renderer.RegisterUIReloadCallback(OnUIReload);
            ShowCore();
        }

        protected override void HideCore()
        {
            gameObject.SetActive(false);
        }

        protected override void ShowCore()
        {
            gameObject.SetActive(true);
        }

        protected virtual void OnInited() { }

        private void OnUIReload(PanelRenderer _, VisualElement root)
        {
            rendererRoot = root;
        }

        private async UniTask InitAsync()
        {
            destroyCancellationToken.ThrowIfCancellationRequested();

            try
            {
                await WaitUntilChildrensInitedAsync();
                await InitVisibleStateAsync();
                OnInited();
                ExecuteOnInitedEvent();
                commandScheduler.Enable();
            }
            catch (System.Exception)
            {
                isInitFaulted = true;
                throw;
            }
            finally
            {
                IsInited = true;
            }
        }
    }
}
