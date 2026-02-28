using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    public record CollectionSnapshot<TCollection, TItem> : Snapshot<TCollection>
        where TCollection : ICollection<TItem>
    {
        public IReadOnlyList<TItem>? Items { get; private set; }

        public CollectionSnapshot()
        {
        }

        public CollectionSnapshot(TCollection target) : base(target)
        {
        }

        protected override void OnRestore(ref TCollection target)
        {
            if (Items.IsNotNull())
            {
                foreach (var item in Items)
                    target.Add(item);
            }
        }

        protected override void OnCapture(TCollection target)
        {
            base.OnCapture(target);

            Items = target.Aggregate(new List<TItem>(),
                (collection, item) =>
                {
                    collection.Add(item);
                    return collection;
                });
        }

        protected override void OnReset()
        {
            base.OnReset();

            Items = null;
        }
    }
}
