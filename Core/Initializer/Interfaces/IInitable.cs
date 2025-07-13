#nullable enable
namespace UTIRLib.Init
{
    public interface IInitable
    {
        bool IsInited { get; }

        void Init();
    }
}
