#nullable enable
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface ISelectable
    {
        IReadOnlyReactiveProperty<bool> IsSelected { get; }

        void Select();

        void Deselect();
    }
}
