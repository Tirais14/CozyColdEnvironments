#if BIN_PACKER_EB_PLUGIN
using CCEnvs.Pools;
using CromulentBisgetti.ContainerPacking;
using CromulentBisgetti.ContainerPacking.Algorithms;
using CromulentBisgetti.ContainerPacking.Entities;
using System.Collections.Generic;
using UnityEngine;
using Item = CromulentBisgetti.ContainerPacking.Entities.Item;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public static class BinPackerED_AFIT
    {
        public static List<Vector3> Pack(
            Vector3 containerSize,
            int entryQuantity,
            Vector3 padding = default
            )
        {
            using var items = ListPool<Item>.Shared.Get();
            var item = CreateItem(entrySize, 0, entryQuantity, padding);
            items.Value.Add(item);

            using var containers = ListPool<Container>.Shared.Get();
            var container = CreateContainer(containerSize, 0);
            containers.Value.Add(container);

            using var algIDs = ListPool<int>.Shared.Get();
            algIDs.Value.Add((int)AlgorithmType.EB_AFIT);

            List<ContainerPackingResult> packingResults = PackingService.Pack(
                containers.Value,
                items.Value,
                algIDs.Value
                );

            var packedItems = packingResults[0].AlgorithmPackingResults[0].PackedItems;

            var packedPositions = ExtractPositions(packedItems);

            return packedPositions;
        }

        public static List<Vector3> ExtractPositions(ICollection<Item> items)
        {
            CC.Guard.IsNotNull(items, nameof(items));

            var positions = new List<Vector3>(items.Count);

            foreach (var item in items)
                positions.Add(ExtractPosition(item));

            return positions;
        }

        public static Vector3 ExtractPosition(Item item)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            return new Vector3(
                (float)item.CoordX,
                (float)item.CoordY,
                (float)item.CoordZ
                );
        }

        public static Item CreateItem(
            Vector3 size,
            int id,
            int quantity = 1,
            Vector3 extraSize = default
            )
        {
            return new Item(
                id,
                (decimal)(size.x + extraSize.x),
                (decimal)(size.y + extraSize.y),
                (decimal)(size.z + extraSize.z),
                quantity
                );
        }

        public static Container CreateContainer(Vector3 size, int id)
        {
            return new Container(
                id,
                (decimal)size.z,
                (decimal)size.x,
                (decimal)size.y
                );
        }
    }
}
#endif
