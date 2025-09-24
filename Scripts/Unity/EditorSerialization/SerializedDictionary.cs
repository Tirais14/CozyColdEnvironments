using CCEnvs.Diagnostics;
using LinqAF;
using SuperLinq;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public sealed class SerializedDictionary<TKey, TValue>
        : Serialized<SerializedTuple<TKey, TValue>[], Dictionary<TKey, TValue>>
    {
        public SerializedDictionary()
        {
        }

        protected override SerializedTuple<TKey, TValue>[] GetInput()
        {
            return Output.Select(x => new SerializedTuple<TKey, TValue>(x.Key, x.Value))
                         .ToArray();
        }

        protected override Dictionary<TKey, TValue> GetOutput()
        {
            return new Dictionary<TKey, TValue>(
                input.Select(x => new KeyValuePair<TKey, TValue>(x.item1, x.item2))
                    .AsEnumerable());
        }

        protected override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();

            try
            {
                input = input?.DistinctBy(pair => pair.item1).ToArray() //by key
                        ?? 
                        Array.Empty<SerializedTuple<TKey, TValue>>();
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }
    }
}
