#nullable enable
namespace UTIRLib.Tickables
{
    public interface ITickableBase : IStateToggleable
    {
        bool IsTickableEnabled { get; set; }

        bool IStateToggleable.IsEnabled {
            get => IsEnabled;
            set => IsEnabled = value;
        }

        void OnRegister()
        {
        }

        void OnUnregister()
        {
        }
    }
    public interface ITickableBase<in T> : ITickableBase
        where T : ITicker
    {
    }
}
