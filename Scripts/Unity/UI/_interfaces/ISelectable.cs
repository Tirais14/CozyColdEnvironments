#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface ISelectable
    {
        bool IsSelected { get; }

        void Select();

        void Deselect();
    }
}
