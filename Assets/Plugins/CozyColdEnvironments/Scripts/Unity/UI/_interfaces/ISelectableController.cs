#nullable enable
using CCEnvs.FuncLanguage;
using System;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface ISelectableController<T>
    {
        Maybe<T> Selection { get; }

        IObservable<PreviousCurrentPair<Maybe<T>, T>> ObserveSelected();

        IObservable<Unit> ObserveDeselected();

        IObservable<PreviousCurrentPair<Maybe<T>>> ObserveSelection();
    }
}
