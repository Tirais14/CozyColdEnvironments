using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using static UnityEditor.Experimental.GraphView.Port;

#nullable enable
namespace CCEnvs.Unity.Items.Snapshots
{
    [Serializable]
    public class ItemContainerSnapshot : Snapshot<ItemContainer>
    {
        [JsonInclude]
		[JsonPropertyName("item")]
        public Maybe<IItem> Item { get; private set; }

        [JsonInclude]
		[JsonPropertyName("itemCount")]
        public int ItemCount { get; private set; }

        [JsonInclude]
		[JsonPropertyName("capacity")]
        public int Capacity { get; private set; }

        [JsonInclude]
		[JsonPropertyName("isReadOnlyContainer")]
        public bool IsReadOnlyContainer { get; private set; }

        [JsonInclude]
		[JsonPropertyName("unlockCapacity")]
        public bool UnlockCapacity { get; private set; }

        public ItemContainerSnapshot()
        {
        }

        public ItemContainerSnapshot(ItemContainer target) : base(target)
        {
            Item = target.Item;
            ItemCount = target.ItemCount;
            Capacity = target.Capacity;
            IsReadOnlyContainer = target.IsReadOnlyContainer;
            UnlockCapacity = target.UnlockCapacity;
        }

        public ItemContainerSnapshot(IItemContainer target)
            :
            this(new ItemContainer(
                item: target.Item.Raw,
                count: target.ItemCount,
                capacity: target.Capacity,
                isReadOnlyContainer: target.IsReadOnlyContainer)
                {
                    UnlockCapacity = target.UnlockCapacity,
                    Capacity = target.Capacity
                })
        {
        }

        public override ItemContainer Restore(ItemContainer target)
        {
            CC.Guard.IsNotNull(target, nameof(target));

            return new ItemContainer(
                item: Item.Raw,
                count: ItemCount,
                isReadOnlyContainer: IsReadOnlyContainer)
            {
                UnlockCapacity = UnlockCapacity,
                Capacity = Capacity,
            };
        }
    }

    public static class ItemContainerSnapshotExtensions
    {
        public static ItemContainerSnapshot CaptureState(this ItemContainer source)
        {
            return new ItemContainerSnapshot(source); 
        }
    }
}
