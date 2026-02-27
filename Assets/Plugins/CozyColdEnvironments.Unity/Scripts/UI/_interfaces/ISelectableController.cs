#nullable enable
using CCEnvs.FuncLanguage;
using R3;

namespace CCEnvs.Unity.UI
{
    public interface ISelectableController
    {
    }
    public interface ISelectableController<T> : ISelectableController
    {
        Maybe<T> Selection { get; }

        void ResetSelection();

        Observable<T> ObserveSelected();

        Observable<T> ObserveDeselected();

        Observable<Maybe<T>> ObserveSelection();
    }
}
