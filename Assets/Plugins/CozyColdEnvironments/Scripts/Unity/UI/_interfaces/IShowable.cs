#nullable enable
using System;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface IShowable
    {
        bool IsVisible { get; }

        void Hide(DisableGraphicsSettings disableGraphicsSettings);
        void Hide();

        void Show();

        bool SwitchVisibleState();

        IObservable<Unit> ObserveShow();

        IObservable<Unit> ObserveHide();
    }
}
