#nullable enable
namespace UTIRLib.AlternativeTicker
{
    public interface ILateTicker : ITickerBase
    {
        void Register(ILateTickable tickable);

        void Unregister(ILateTickable tickable);
    }
}
