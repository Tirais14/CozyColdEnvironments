using System;
using System.Collections.Generic;
using System.Linq;
using CCEnvs.Collections;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    public class DictionarySnapshot<TKey, TValue> : Snapshot<IDictionary<TKey, TValue>>
    {
        public HashSet<KeyValuePair<TKey, TValue>>? Pairs { get; set; }

        public DictionarySnapshot()
        {
        }

        public DictionarySnapshot(IDictionary<TKey, TValue> target) : base(target)
        {
            Pairs = target.ToHashSet();
        }

        public override bool CanRestore(IDictionary<TKey, TValue>? target)
        {
            return target.IsNotNullOrEmpty() && Pairs.IsNotNullOrEmpty();
        }

        protected override IDictionary<TKey, TValue>? CreateValue()
        {
            return new Dictionary<TKey, TValue>(Pairs?.Count ?? 4);
        }

        protected override void OnRestore(ref IDictionary<TKey, TValue> target)
        {
            foreach (var pair in Pairs!)
            {
                if (pair.Key is null)
                    continue;

                if (target.ContainsKey(pair.Key))
                    target[pair.Key] = pair.Value;
                else
                    target.Add(pair.Key, pair.Value);
            }
        }
    }
}
