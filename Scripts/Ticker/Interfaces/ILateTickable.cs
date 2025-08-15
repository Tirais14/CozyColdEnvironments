#nullable enable
namespace UTIRLib.Tickables
{
    public interface ILateTickable : ITickableBase
    {
        void LateTick();
    }
}
