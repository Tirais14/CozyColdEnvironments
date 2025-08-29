#nullable enable
namespace CozyColdEnvironments.Tickables
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
