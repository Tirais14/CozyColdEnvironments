#nullable enable
using System;
using R3;
using UnityEngine;

namespace CCEnvs.Unity.UI
{
    public interface IShowable
    {
        bool IsShown { get; }
        bool IsInited { get; }

        void Hide();
 
        void Show();

        void Redraw();

        bool SwitchShownState();

        void SwitchShownStateVoid();

        Observable<Unit> ObserveShow();

        Observable<Unit> ObserveHide();
    }
}
