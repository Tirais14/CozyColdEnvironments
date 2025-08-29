#nullable enable
namespace CozyColdEnvironments.Tickables
{
    public interface ILateTickable : ITickableBase
    {
        void DoLateTick();
    }
}
