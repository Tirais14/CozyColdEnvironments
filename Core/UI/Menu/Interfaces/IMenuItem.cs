#nullable enable

namespace UTIRLib.UI
{
    public interface IMenuItem
    {
        bool IsEnabled { get; }

        void Show();

        void Hide();

        bool IsSupportedObject(object? obj);
    }
}