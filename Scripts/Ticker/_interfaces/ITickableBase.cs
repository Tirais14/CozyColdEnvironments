#nullable enable
namespace UTIRLib.Tickables
{
    public interface ITickableBase : IStateToggleable
    {
        void OnRegister(ITickerBase ticker)
        {
        }

        void OnUnregister(ITickerBase ticker)
        {
        }
    }
    public interface ITickableBase<in T> : ITickableBase
        where T : ITickerBase
    {
        void OnRegister(T ticker)
        {
        }

        void OnUnregister(T ticker)
        {
        }

        void ITickableBase.OnRegister(ITickerBase ticker)
        {
            if (ticker is not T typed)
                throw new System.ArgumentException(nameof(ticker));

            OnRegister(typed);
        }

        void ITickableBase.OnUnregister(ITickerBase ticker)
        {
            if (ticker is not T typed)
                throw new System.ArgumentException(nameof(ticker));

            OnUnregister(typed);
        }
    }
}
