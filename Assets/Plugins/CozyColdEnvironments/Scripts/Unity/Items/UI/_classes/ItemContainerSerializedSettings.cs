using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.Unity.Storages.UI;
using CCEnvs.Unity.UI;

namespace CCEnvs.Unity.Items.UI
{
    public class ItemContainerSerializedSettings : ViewComponentCommand
    {
        public Maybe<CompareAction<int>> ShowItemCounterPredicate;

        protected override void Start()
        {
            base.Start();

            PreUpdateAction(() =>
            {
                var viewType = view.GetType();

                if (viewType.IsGenericType
                    &&
                    viewType.GetGenericTypeDefinition().IsType(typeof(IItemContainerView<,>)))
                {
                    view.viewModel.Reflect()
                        .Name(nameof(IItemContainerViewModel<ItemContainer>.ShowCounterTextPredicate))
                        .Property()
                        .Strict()
                        .SetValue(view.viewModel, ShowItemCounterPredicate);
                }
            });
        }
    }
}
