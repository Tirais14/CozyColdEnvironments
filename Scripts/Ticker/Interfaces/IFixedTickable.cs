#nullable enable
namespace UTIRLib.Tickables
{
    public interface IFixedTickable : ITickableBase
    {
        void FixedTick();
    }
}
