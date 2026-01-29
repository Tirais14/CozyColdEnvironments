#nullable enable
namespace CCEnvs.Unity.Tickables
{
    public interface ILateTickable : ITickableBase
    {
        void DoLateTick();
    }
}
