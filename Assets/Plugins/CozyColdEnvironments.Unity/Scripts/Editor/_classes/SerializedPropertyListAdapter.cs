#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

#nullable enable
namespace CCEnvs.Unity.UnityEditor
{
    public class SerializedPropertyListAdapter : IList
    {
        public SerializedProperty Property { get; }

        public object this[int index] {
            get => Property.GetArrayElementAtIndex(index);
            set
            {
                throw new NotSupportedException();
            }
        }

        public bool IsFixedSize => true;
        public bool IsReadOnly => false;
        public bool IsSynchronized => true;

        public int Count => Property.arraySize;

        public object SyncRoot { get; } = new();

        public SerializedPropertyListAdapter(SerializedProperty prop)
        {
            this.Property = prop;
        }

        public bool Contains(object value)
        {
            foreach (var item in this)
            {
                if (EqualityComparer<object>.Default.Equals(item, value))
                    return true;
            }

            return false;
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return Property.GetArrayElementAtIndex(i);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        int IList.IndexOf(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }
    }
}
#endif