using CCEnvs.Unity.GameSystems.Storages;
using System;
using System.Collections.Generic;
using UniRx;

#nullable enable
namespace CCEnvs.Unity
{
    public interface IInventory
        : IItemAccessor,
        IItemContainerInfoItemless,
        IReadOnlyDictionary<int, IItemContainer>,
        IReadOnlyReactiveDictionary<int, IItemContainer>
    {
        void Add(int id, IItemContainer itemContainer);

        bool Remove(int id);
        bool Remove(IItemContainer itemContainer);

        bool Contains(IItemContainer itemContainer);
    }
}
