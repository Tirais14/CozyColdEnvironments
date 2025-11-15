using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI.MVVM;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public interface IItemContainerView<TViewModel, TContainer>
        : IView<TViewModel, TContainer>

        where TViewModel : IItemContainerViewModel<TContainer>
        where TContainer : IItemContainerInfo
    {
    
    }
}
