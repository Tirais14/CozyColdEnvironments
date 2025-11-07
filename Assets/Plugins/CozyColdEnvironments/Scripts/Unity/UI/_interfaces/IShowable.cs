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
            Default = None,
        }

        bool IsShowAllowed { get; }
        bool IsVisible { get; }

        void Hide(Settings settings);
        void Hide();

        void Show(Settings settings);   
        void Show();

        bool SwitchVisibleState();

        IObservable<Unit> ObserveShow();

        IObservable<Unit> ObserveHide();
    }
}
