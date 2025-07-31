#nullable enable
namespace UTIRLib.UI
{
    public interface ISelectable
    {
        bool IsSelected { get; }

        void Select();

        void Deselect();
    }
}
