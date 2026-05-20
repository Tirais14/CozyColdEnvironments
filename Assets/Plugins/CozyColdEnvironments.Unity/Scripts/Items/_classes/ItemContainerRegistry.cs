using CCEnvs.Disposables;
using CCEnvs.TypeMatching;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public static class ItemContainerRegistry
    {
        public static IReadOnlyDictionary<int, IItemContainerInfo> ItemContainers => itemContainers;

        private static readonly Dictionary<int, IItemContainerInfo> itemContainers = new();

        public static LightDisposable<int> Register(int id, IItemContainerInfo itemContainer)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            itemContainers.Add(id, itemContainer);

            return CCDisposable.CreateLight(id, static (id) => Unregister(id));
        }

        public static bool Unregister(int id)
        {
            return itemContainers.Remove(id);
        }

        public static IItemContainerInfo Get(int id) => itemContainers[id];
        public static IItemContainerInfo Get<T>(int id)
            where T: IItemContainerInfo
        {
            return (T)itemContainers[id];
        }

        public static bool TryGet(
            int id,
            [NotNullWhen(true)] out IItemContainerInfo? itemContainer
            )
        {
            return itemContainers.TryGetValue(id, out itemContainer);    
        }
        public static bool TryGet<T>(int id, [NotNullWhen(true)] out T? itemContainer)
            where T : IItemContainerInfo
        {
            if (!TryGet(id, out var itemContainerUntyped)
                ||
                itemContainerUntyped.IsNot<T>(out itemContainer))
            {
                itemContainer = default;
                return false;
            }

            return true;
        }
    }
}
