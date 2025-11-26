#nullable enable
using System;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface IShowable
    {
        bool IsVisible { get; }
        bool ShowAllowed { get; }
        bool ShowOnInited { get; set; }

        void Hide();
 
        void Show();

        void Redraw();

        bool SwitchVisibleState();

        void SwitchVisibleStateVoid();

        IObservable<Unit> ObserveShow();

        IObservable<Unit> ObserveHide();
    }
}
