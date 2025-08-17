#nullable enable
using System.Reflection;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection.Cached;

namespace UTIRLib.Tickables
{
    public interface ITickableBase : IStateToggleable
    {
        bool IsTickableEnabled { get; set; }

        bool IStateToggleable.IsEnabled {
            get => IsEnabled;
            set => IsEnabled = value;
        }

        void OnRegister(ITickerBase ticker)
        {
            FieldInfo? tickerField = TypeCache.GetField(typeof(ITickerBase),
                                                        typeof(ITickerBase),
                                                        BindingFlagsDefault.InstanceAll,
                                                        throwIfNotFound: false);

            if (tickerField is not null && tickerField.GetValue(this).IsNull())
                tickerField.SetValue(this, ticker);
        }

        void OnUnregister(ITickerBase ticker)
        {
            FieldInfo? tickerField = TypeCache.GetField(typeof(ITickerBase),
                                                        typeof(ITickerBase),
                                                        BindingFlagsDefault.InstanceAll,
                                                        throwIfNotFound: false);

            tickerField?.SetValue(this, null);
        }
    }
    public interface ITickableBase<in T> : ITickableBase
        where T : ITickerBase
    {
        void OnRegister(T ticker)
        {
            FieldInfo? tickerField = TypeCache.GetField(typeof(ITickerBase),
                                                        typeof(ITickerBase),
                                                        BindingFlagsDefault.InstanceAll,
                                                        throwIfNotFound: false);

            if (tickerField is not null && tickerField.GetValue(this).IsNull())
                tickerField.SetValue(this, ticker);
        }

        void OnUnregister(T ticker)
        {
            FieldInfo? tickerField = TypeCache.GetField(typeof(ITickerBase),
                                                        typeof(ITickerBase),
                                                        BindingFlagsDefault.InstanceAll,
                                                        throwIfNotFound: false);

            tickerField?.SetValue(this, null);
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
