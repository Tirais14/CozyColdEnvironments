#nullable enable
using System;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface IShowable
    {
        bool IsVisible { get; }
        bool ShowAllowed { get; }

        void Hide();
 
        void Show();

        void Redraw();

        bool SwitchVisibleState();

        IObservable<Unit> ObserveShow();

        IObservable<Unit> ObserveHide();
    }
}
