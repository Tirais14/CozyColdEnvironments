using UniRx;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.UI.ItemStorage
{
    public class ItemStackView<TViewModel, TModel>  : AViewLazy<TViewModel>
        where TViewModel : IViewModel<TModel>
        where TModel : IItemStackReactive
    {
        protected override TViewModel CreateViewModel()
        {
            TModel model = CreateModel();

            var creationParams = new ConstructorParameters
            {
                BindingFlags = BindingFlagsDefault.InstancePublic,
                ArgumentsData = InvokableArguments.Create(model)
            };

            return InstanceFactory.Create<TViewModel>(creationParams,
                                                      cacheResults: true);
        }

        private TModel CreateModel()
        {
            var creationParams = new ConstructorParameters
            {
                BindingFlags = BindingFlagsDefault.InstancePublic,
                ArgumentsData = InvokableArguments.Create(int.MaxValue)
            };

            if (!InstanceFactory.TryCreate<TModel>(creationParams,
                                                   cacheResult: true,
                                                   out var model))
            {
                model = InstanceFactory.Create<TModel>(creationParams with
                {
                    ArgumentsData = InvokableArguments.Empty
                },
                cacheResults: true);
            }

            return model;
        }
    }
}
