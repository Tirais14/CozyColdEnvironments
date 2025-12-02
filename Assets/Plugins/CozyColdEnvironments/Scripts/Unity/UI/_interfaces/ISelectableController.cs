#nullable enable
using CCEnvs.FuncLanguage;
using System;

namespace CCEnvs.Unity.UI
{
    public interface ISelectableController<T>
    {
        Maybe<T> Selection { get; }

        IObservable<T> ObserveSelected();

        IObservable<T> ObserveDeselected();

        IObservable<PreviousCurrentPair<Maybe<T>>> ObserveSelection();
    }
}
