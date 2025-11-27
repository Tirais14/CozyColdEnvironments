#nullable enable
using System;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface IShowable
    {
        bool IsShown { get; }
        bool IsVisible { get; }
        bool ShowAllowed { get; }
        bool HideAllowed { get; }
        bool ShowOnInited { get; set; }

        void Hide();
 
        void Show();

        void Redraw();

        bool SwitchShownState();

        void SwitchShownStateVoid();

        IObservable<Unit> ObserveShow();

        IObservable<Unit> ObserveHide();
    }
}
