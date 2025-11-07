#nullable enable
using System;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface IShowable
    {
        bool IsVisible { get; }

        void Hide(ShowableSettings settings);
        void Hide();

        void Show(ShowableSettings settings);   
        void Show();

        bool SwitchVisibleState();

        IObservable<Unit> ObserveShow();

        IObservable<Unit> ObserveHide();
    }
}
