using CCEnvs.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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

        public override bool CanRestore([NotNull] IDictionary<TKey, TValue>? target)
        {
            return target.IsNotNullOrEmpty() && Pairs.IsNotNullOrEmpty();
        }

        public override bool TryRestore(
            IDictionary<TKey, TValue>? target, 
            [NotNullWhen(true)] out IDictionary<TKey, TValue>? restored
            )
        {
            if (!CanRestore(target))
            {
                restored = null;
                return false;
            }

            if (target is null)
                restored = new Dictionary<TKey, TValue>(Pairs!.Count);
            else
                restored = target;

            foreach (var pair in Pairs!)
            {
                if (pair.Key is null)
                    continue;

                if (restored.ContainsKey(pair.Key))
                    restored[pair.Key] = pair.Value;
                else
                    restored.Add(pair.Key, pair.Value);
            }
            
            return true;
        }
    }
}
