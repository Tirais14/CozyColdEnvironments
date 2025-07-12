#nullable enable
namespace UTIRLib.AlternativeTicker
{
    public interface ITickable : ITickableBase
    {
        void Tick();
    }
}
