#nullable enable
namespace CCEnvs.UnityX.Items.UIElements
{
    public class ItemContainerViewModel<TModel> : UI.ItemContainerViewModel<TModel>
        where TModel : IItemContainer
    {
        public ItemContainerViewModel()
        {
        }

        public ItemContainerViewModel(TModel? model) : base(model)
        {
        }
    }
}
