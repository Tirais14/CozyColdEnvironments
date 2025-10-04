#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface IShowable
    {
        bool IsVisible { get; }

        void Show();

        void Hide();
    }
}
