#nullable enable
using Cysharp.Threading.Tasks;
using R3;
using System.Threading;

namespace CCEnvs.Unity.UI
{
    public interface IShowable
    {
        bool IsShown { get; }

        bool IsInited { get; }

        bool IsReadyToShow { get; }

        UniTask WaitForInitializedAsync(CancellationToken cancellationToken = default);

        void Hide();

        UniTask HideAsync(CancellationToken cancellationToken = default);
 
        void Show();

        UniTask ShowAsync(CancellationToken cancellationToken = default);

        void Redraw();

        UniTask RedrawAsync(CancellationToken cancellationToken = default);

        bool SwitchShownState();

        UniTask<bool> SwitchShownStateAsync(CancellationToken cancellationToken = default);

        void SwitchShownStateVoid();

        Observable<bool> ObserveIsInited();

        Observable<bool> ObserveShow();

        Observable<bool> ObserveHide();
    }
}
