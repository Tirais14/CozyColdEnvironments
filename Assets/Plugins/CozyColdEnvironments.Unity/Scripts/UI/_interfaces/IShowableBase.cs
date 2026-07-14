using Cysharp.Threading.Tasks;
using R3;
using System.Threading;

#nullable enable
namespace CCEnvs.UnityX.UI
{
    public interface IShowableBase
    {
        bool IsShown { get; }

        bool IsInited { get; }

        bool IsReadyToShow { get; }

        bool IsEnabled { get; set; }

        bool PreventHide { get; set; }

        UniTask WaitUntilInited(CancellationToken cancellationToken = default);

        void Hide();

        UniTask HideAsync(CancellationToken cancellationToken = default);

        void Show();

        UniTask ShowAsync(CancellationToken cancellationToken = default);

        bool SwitchShownState();

        UniTask<bool> SwitchShownStateAsync(CancellationToken cancellationToken = default);

        void SwitchShownStateVoid();

        void Redraw();

        UniTask RedrawAsync(CancellationToken cancellationToken = default);

        T[] GetChilds<T>();

        Observable<bool> ObserveIsInited();

        Observable<bool> ObserveShow();

        Observable<bool> ObserveHide();
    }
}
