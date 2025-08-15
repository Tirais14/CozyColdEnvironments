#nullable enable
namespace UTIRLib.Tickables
{
    public interface ITickable : ITickableBase
    {
        void DoTick();
    }
}
