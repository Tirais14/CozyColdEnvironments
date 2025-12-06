#nullable enable
using CCEnvs.FuncLanguage;
using System;

namespace CCEnvs.Unity.UI
{
    public interface ISelectableController<T>
    {
        Maybe<T> Selection { get; }

        void ResetSelection();

        IObservable<T> ObserveSelected();

        IObservable<T> ObserveDeselected();

        IObservable<PreviousCurrentPair<Maybe<T>>> ObserveSelection();
    }
}
