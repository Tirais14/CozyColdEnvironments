using CCEnvs.Unity.Injections;
using Cysharp.Threading.Tasks;
using R3;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public partial class GUITab : IShowable
    {
        [field: GetBySelf]
        protected Showable showable { get; private set; } = null!;

        public bool IsShown => showable.IsShown;
        public bool IsInited => showable.IsInited;
        public bool IsReadyToShow => showable.IsReadyToShow;

        public bool IsEnabled {
            get => showable.enabled;
            set => showable.enabled = value;
        }

        public bool PreventHide {
            get => showable.PreventHide;
            set => showable.PreventHide = value;
        }

        public Graphic? graphic => showable.graphic;

        public Image? image => showable.image;

        public CanvasGroup? canvasGroup => showable.canvasGroup;

        public Canvas canvas => showable.canvas;

        public IShowable? root => showable.root;
        public IShowable? parent => showable.parent;

        public ICanvasController? canvasController => showable.canvasController;

        public ShowableRenderMode RenderMode {
            get => showable.RenderMode;
            set => showable.RenderMode = value;
        }

        public T[] GetChilds<T>()
        {
            return showable.GetChilds<T>();
        }

        public IShowable[] GetDirectChilds()
        {
            return showable.GetDirectChilds();
        }

        public void Hide()
        {
            showable.Hide();
        }

        public async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            await showable.HideAsync(cancellationToken);
        }

        public void Redraw()
        {
            showable.Redraw();
        }

        public async UniTask RedrawAsync(CancellationToken cancellationToken = default)
        {
            await showable.RedrawAsync(cancellationToken);
        }

        public void Show()
        {
            showable.Show();
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            await showable.ShowAsync(cancellationToken);
        }

        public bool SwitchShownState()
        {
            return showable.SwitchShownState();
        }

        public async UniTask<bool> SwitchShownStateAsync(CancellationToken cancellationToken = default)
        {
            return await showable.SwitchShownStateAsync(cancellationToken);
        }

        public void SwitchShownStateVoid()
        {
            showable.SwitchShownStateVoid();
        }

        public async UniTask WaitUntilInited(CancellationToken cancellationToken = default)
        {
            await showable.WaitUntilInited(cancellationToken);
        }

        public Observable<bool> ObserveHide()
        {
            return showable.ObserveHide();
        }

        public Observable<bool> ObserveIsInited()
        {
            return showable.ObserveIsInited();
        }

        public Observable<bool> ObserveShow()
        {
            return showable.ObserveShow();
        }
    }
}
