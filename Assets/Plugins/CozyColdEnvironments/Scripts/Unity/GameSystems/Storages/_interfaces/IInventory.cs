using CCEnvs.Unity.GameSystems.Storages;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity
{
    public interface IInventory
        : IItemAccessor,
        IItemContainerInfo,
        IReadOnlyDictionary<int, IItemContainer>
    {
        void Add(int id, IItemContainer itemContainer);

        void Remove(int id);
    }
}
