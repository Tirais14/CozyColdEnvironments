#nullable enable
namespace CozyColdEnvironments.UI
{
    public interface IOpenable
    {
        bool IsOpened { get; }

        void Open();

        void Close();

        bool SwitchOpenableState();
    }
}