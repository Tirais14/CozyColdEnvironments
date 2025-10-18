using CCEnvs.Unity.GameSystems.Storages;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity
{
    public interface IInventory
        : IItemAccessor,
        IItemContainerInfoItemless,
        IReadOnlyDictionary<int, IItemContainer>
    {


        void Add(int id, IItemContainer itemContainer);

        void Remove(int id);

        bool Contains(IItemContainer itemContainer);
    }
}
