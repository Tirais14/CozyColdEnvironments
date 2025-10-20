using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.UI.Elements;

namespace CCEnvs.Unity.UI.Storages
{
    public class ItemContainerList<T> : ListElement<T>
        where T : IItemContainer
    {
    }
    public class ItemContainerList : ItemContainerList<IItemContainer>
    {
    }
}
