#nullable enable
using Cysharp.Threading.Tasks;
using R3;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace CCEnvs.Unity.UI
{
    public interface IShowable
    {
        bool IsShown { get; }

        bool IsInited { get; }

        bool IsReadyToShow { get; }

        bool IsEnabled { get; set; }

        bool PreventHide { get; set; }

        Graphic? graphic { get; }

        Image? image { get; }

        CanvasGroup? canvasGroup { get; }

        Canvas canvas { get; }

        IShowable? root { get; }

        IShowable? parent { get; }

        ICanvasController? canvasController { get; }

        ShowableRenderMode RenderMode { get; set; }

        UniTask WaitUntilInited(CancellationToken cancellationToken = default);

        void Hide();

        UniTask HideAsync(CancellationToken cancellationToken = default);

        void Show();

        UniTask ShowAsync(CancellationToken cancellationToken = default);

        void Redraw();

        UniTask RedrawAsync(CancellationToken cancellationToken = default);

        bool SwitchShownState();

        UniTask<bool> SwitchShownStateAsync(CancellationToken cancellationToken = default);

        void SwitchShownStateVoid();

        IShowable[] GetDirectChilds();

        T[] GetChilds<T>();

        Observable<bool> ObserveIsInited();

        Observable<bool> ObserveShow();

        Observable<bool> ObserveHide();
    }
}
