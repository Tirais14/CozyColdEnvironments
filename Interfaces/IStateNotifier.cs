using System;

#nullable enable

namespace UTIRLib
{
    [Obsolete]
    public interface IStateNotifier
    {
        event Action OnStateChanged;
    }

    [Obsolete]
    public interface IStateNotifier<T>
    {
        event Action<T> OnStateChanged;
    }
}