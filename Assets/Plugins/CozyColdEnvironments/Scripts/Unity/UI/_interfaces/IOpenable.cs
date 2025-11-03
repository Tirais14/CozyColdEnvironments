using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface IOpenable
    {
        IReactiveProperty<bool> IsOpened { get; }

        void Open();

        void Close();

        void SwitchOpenState();
    }
}
