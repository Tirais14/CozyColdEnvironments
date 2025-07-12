using System;

#nullable enable

namespace UTIRLib
{
    public interface IStateNotifier
    {
        event Action OnStateChanged;
    }

    public interface IStateNotifier<T>
    {
        event Action<T> OnStateChanged;
    }
}