using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace CCEnvs.Snapshots
{
    public class CollectionSnapshot<TCollection, TItem> : Snapshot<TCollection>
        where TCollection : ICollection<TItem>
    {
        public IReadOnlyList<TItem> Items { get; private set; } = Array.Empty<TItem>();

        public CollectionSnapshot()
        {
        }

        public CollectionSnapshot(TCollection target) : base(target)
        {
            Items = target.Aggregate(new List<TItem>(),
                (collection, item) =>
                {
                    collection.Add(item);
                    return collection;
                });
        }

        protected override void OnRestore(ref TCollection target)
        {
            foreach (var item in Items)
                target.Add(item);
        }
    }
}
