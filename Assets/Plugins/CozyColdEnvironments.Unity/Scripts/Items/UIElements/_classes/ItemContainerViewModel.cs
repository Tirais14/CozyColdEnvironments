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

    public class ItemContainerViewModel : ItemContainerViewModel<IItemContainer>
    {
        public ItemContainerViewModel()
        {
        }

        public ItemContainerViewModel(IItemContainer? model) : base(model)
        {
        }
    }
}
