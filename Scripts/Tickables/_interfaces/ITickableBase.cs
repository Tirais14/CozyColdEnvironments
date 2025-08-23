#nullable enable
namespace UTIRLib.Tickables
{
    public interface ITickableBase
    {
        bool IsTickableEnabled { get; set; }

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
