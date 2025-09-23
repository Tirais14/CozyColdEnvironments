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
            :
            base(input => new Dictionary<TKey, TValue>(input.Select(x => x.ToKeyValuePair()).AsEnumerable()),
                 output => output.Select(x => x.ToTuple().ToSerializedTuple()).ToArray())
        {
        }

        protected override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();

            try
            {
                input = input?.DistinctBy(pair => pair.item1).ToArray()!; //by key
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }
    }
}
