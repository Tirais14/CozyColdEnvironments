#nullable enable
using CCEnvs.FuncLanguage;
using System;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface IShowable
    {
        [Flags]
        public enum Settings
        {
            None = 0,
            KeepRaycastTargetState = 1,
            Recursive = 2,
            ByComponentState = 4,
            Default = Recursive | ByComponentState,
        }
        bool IsVisible { get; }

        void Hide(Settings settings);
        void Hide();

        void Show(Settings settings);   
        void Show();

        void Redraw();

        bool SwitchVisibleState();

        IObservable<Unit> ObserveShow();

        IObservable<Unit> ObserveHide();
    }
}
