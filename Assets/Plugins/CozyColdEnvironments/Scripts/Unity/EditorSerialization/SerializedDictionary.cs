using CCEnvs.Diagnostics;
using ZLinq;
using SuperLinq;
using System;
using System.Collections.Generic;
using CCEnvs.Linq;
using CCEnvs;

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

        protected override SerializedTuple<TKey, TValue>[] ConvertToInput(Dictionary<TKey, TValue> output)
        {
            return Value.AsValueEnumerable()
                        .Select(x => new SerializedTuple<TKey, TValue>(x.Key, x.Value))
                        .ToArray();
        }

        protected override Dictionary<TKey, TValue> ConvertToOutput(
            SerializedTuple<TKey, TValue>[] input)
        {
            return new Dictionary<TKey, TValue>(
                input.ZL().Select(x => x.Value.ToKeyValuePair()).ToArrayPool().Array);
        }

        protected override void OnBeforeSerialize()
        {
            try
            {
                input = input.AsValueEnumerable().DistinctBy(pair => pair.Value.Item1).ToArray() //by key
                        ??
                        Array.Empty<SerializedTuple<TKey, TValue>>();
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }

            base.OnBeforeSerialize();
        }
    }
}
