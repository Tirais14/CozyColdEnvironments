using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    public static class SerializedTuple
    {
        public static SerializedTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new SerializedTuple<T1, T2>(item1, item2);
        }
        public static SerializedTuple<T1, T2, T3> Create<T1, T2, T3>(
            T1 item1,
            T2 item2,
            T3 item3)
        {
            return new SerializedTuple<T1, T2, T3>(
                item1,
                item2,
                item3);
        }
        public static SerializedTuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4)
        {
            return new SerializedTuple<T1, T2, T3, T4>(
                item1,
                item2,
                item3,
                item4);
        }
        public static SerializedTuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5)
        {
            return new SerializedTuple<T1, T2, T3, T4, T5>(
                item1,
                item2,
                item3,
                item4,
                item5);
        }
        public static SerializedTuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6)
        {
            return new SerializedTuple<T1, T2, T3, T4, T5, T6>(
                item1,
                item2,
                item3,
                item4,
                item5,
                item6);
        }
    }
    [Serializable]
    public struct SerializedTuple<T1, T2> : IEditorSerialized<(T1, T2)>
    {
        [SerializeField]
        private T1 item1;

        [SerializeField]
        private T2 item2;

        public readonly (T1, T2) Value => (item1, item2);

        public SerializedTuple(T1 item1, T2 item2)
        {
            this.item1 = item1;
            this.item2 = item2;
        }

        public static implicit operator (T1, T2)(SerializedTuple<T1, T2> source)
        {
            return source.Value;
        }
    }
    [Serializable]
    public struct SerializedTuple<T1, T2, T3> : IEditorSerialized<(T1, T2, T3)>
    {
        [SerializeField]
        private T1 item1;

        [SerializeField]
        private T2 item2;

        [SerializeField]
        private T3 item3;

        public readonly (T1, T2, T3) Value => (item1, item2, item3);

        public SerializedTuple(T1 item1, T2 item2, T3 item3)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
        }

        public static implicit operator (T1, T2, T3)(SerializedTuple<T1, T2, T3> source)
        {
            return source.Value;
        }
    }
    [Serializable]
    public struct SerializedTuple<T1, T2, T3, T4> : IEditorSerialized<(T1, T2, T3, T4)>
    {
        [SerializeField]
        private T1 item1;

        [SerializeField]
        private T2 item2;

        [SerializeField]
        private T3 item3;

        [SerializeField]
        private T4 item4;

        public readonly (T1, T2, T3, T4) Value => (item1, item2, item3, item4);

        public SerializedTuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
        }

        public static implicit operator (T1, T2, T3, T4)(
            SerializedTuple<T1, T2, T3, T4> source)
        {
            return source.Value;
        }
    }
    [Serializable]
    public struct SerializedTuple<T1, T2, T3, T4, T5> : IEditorSerialized<(T1, T2, T3, T4, T5)>
    {
        [SerializeField]
        private T1 item1;

        [SerializeField]
        private T2 item2;

        [SerializeField]
        private T3 item3;

        [SerializeField]
        private T4 item4;

        [SerializeField]
        private T5 item5;

        public readonly (T1, T2, T3, T4, T5) Value => (item1, item2, item3, item4, item5);

        public SerializedTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
            this.item5 = item5;
        }

        public static implicit operator (T1, T2, T3, T4, T5)(
            SerializedTuple<T1, T2, T3, T4, T5> source)
        {
            return source.Value;
        }
    }
    [Serializable]
    public struct SerializedTuple<T1, T2, T3, T4, T5, T6> : IEditorSerialized<(T1, T2, T3, T4, T5, T6)>
    {
        [SerializeField]
        private T1 item1;

        [SerializeField]
        private T2 item2;

        [SerializeField]
        private T3 item3;

        [SerializeField]
        private T4 item4;

        [SerializeField]
        private T5 item5;

        [SerializeField]
        private T6 item6;

        public readonly (T1, T2, T3, T4, T5, T6) Value => (item1, item2, item3, item4, item5, item6);

        public SerializedTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
            this.item5 = item5;
            this.item6 = item6;
        }

        public static implicit operator (T1, T2, T3, T4, T5, T6)(
            SerializedTuple<T1, T2, T3, T4, T5, T6> source)
        {
            return source.Value;
        }
    }
}
