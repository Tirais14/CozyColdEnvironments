#nullable enable
namespace UTIRLib.AlternativeTicker
{
    public interface IFixedTickable : ITickableBase
    {
        void FixedTick();
    }
}
