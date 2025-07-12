#nullable enable
namespace UTIRLib.AlternativeTicker
{
    public interface ILateTickable : ITickableBase
    {
        void LateTick();
    }
}
