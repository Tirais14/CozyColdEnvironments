#nullable enable
using System;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface IShowable
    {
        event Action OnShow;
        event Action OnHide;

        IReadOnlyReactiveProperty<bool> IsVisible { get; }
        bool IsShowable { get; }

        void Hide();

        void Show();

        bool SwitchVisibleState();

        IObservable<Unit> ObserveShow();

        IObservable<Unit> ObserveHide();
    }
}
