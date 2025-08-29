#nullable enable
namespace CozyColdEnvironments.UI
{
    public interface ISelectable
    {
        bool IsSelected { get; }

        void Select();

        void Deselect();
    }
}
