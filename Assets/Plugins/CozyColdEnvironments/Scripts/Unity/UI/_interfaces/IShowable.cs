#nullable enable
using System;
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.UI
{
    public interface IShowable
    {
        bool IsShown { get; }
        bool ShowAllowed { get; }
        bool HideAllowed { get; }
        bool ShowOnInited { get; set; }
        bool IsInited { get; }

        void Hide();
 
        void Show();

        void Redraw();

        bool SwitchShownState();

        void SwitchShownStateVoid();

        IObservable<Unit> ObserveShow();

        IObservable<Unit> ObserveHide();
    }
}
