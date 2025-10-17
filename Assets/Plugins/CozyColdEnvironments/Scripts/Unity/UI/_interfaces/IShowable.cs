#nullable enable
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface IShowable
    {
        IReadOnlyReactiveProperty<bool> IsVisible { get; }

        void Show();

        void Hide();
    }
}
