using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public struct SerializedTuple<T1, T2> : IEditorSerialized<(T1, T2)>
    {
        [SerializeField]
        public T1 item1;

        [SerializeField]
        public T2 item2;

        public readonly (T1, T2) Output => (item1, item2);

        public SerializedTuple(T1 item1, T2 item2)
        {
            this.item1 = item1;
            this.item2 = item2;
        }


        public static implicit operator KeyValuePair<T1, T2>(SerializedTuple<T1, T2> source)
        {
            return source.ToKeyValuePair();
        }

        public readonly void Deconstruct(out T1 item1, out T2 item2)
        {
            item1 = this.item1; 
            item2 = this.item2;
        }

        public readonly KeyValuePair<T1, T2> ToKeyValuePair()
        {
            return new KeyValuePair<T1, T2>(item1, item2);
        }
    }
}
