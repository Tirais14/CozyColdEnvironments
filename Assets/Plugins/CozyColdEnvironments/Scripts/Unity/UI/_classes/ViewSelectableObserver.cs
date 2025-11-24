using CCEnvs.Unity.UI;
using System.Linq;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity
{
    public class ViewSelectableObserver<T, TModel> : SelectableObserver<T>
        where T : ISelectable
    {
        protected override void CollectSelectables()
        {
            selectables.Clear();
            foreach (var view in from view in this.QueryTo().ByChildren().Views().ZL()
                                 where view is T && view.viewModel.IsSome && view.viewModelUnsafe.model is TModel
                                 select view.As<T>())
            {
                selectables.Add(view);
            }
        }
    }
}
