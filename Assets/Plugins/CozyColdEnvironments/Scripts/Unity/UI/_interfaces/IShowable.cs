#nullable enable
using CCEnvs.Reflection;
using CCEnvs.Unity.UI.Elements;
using System;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface IShowable
    {
        event Action OnShow;
        event Action OnHide;

        IReadOnlyReactiveProperty<bool> IsVisible { get; }
        IObservable<Unit> OnShowRx { get; }
        IObservable<Unit> OnHideRx { get; }

        void Hide();

        void Show();

        bool CanShow(out string msg)
        {
            if (IsVisible.Value)
            {
                msg = $"{GetType().GetFullName()}/> Already opened";
                return false;
            }

            msg = string.Empty;
            return true;
        }
        bool CanShow() => CanShow(out _);

        bool SwitchVisibleState();
    }
}
