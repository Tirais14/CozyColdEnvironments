#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface IOpenable
    {
        bool IsOpened { get; }

        void Open();

        void Close();

        bool SwitchOpenableState();
    }
}