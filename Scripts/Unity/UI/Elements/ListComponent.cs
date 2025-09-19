using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.tvOS;

#nullable enable
namespace CCEnvs.Unity.UI
{
    /// <summary>
    /// Ignores real indices of components
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListComponent<T> : MonoCC, IList<T>, ITrimmable
        where T : Component
    {
        private readonly List<T> inner = new();

        new private Transform transform = null!;

        public int Count => inner.Count;
        public T this[int index] {
            get => inner[index];
            set => inner[index] = value;
        }
        /// <summary>
        /// Destroys <see cref="{T}"/>.<see cref="GameObject"/> instead of <see cref="{T}"/>
        /// </summary>
        public bool DestroyGameObject { get; set; }

        bool ICollection<T>.IsReadOnly => false;

        protected override void OnAwake()
        {
            base.OnAwake();
            transform = base.transform;
        }

        public void Add(T item)
        {
            CC.Validate.ArgumentNull(item, nameof(item));

            inner.Add(item);
            AddInternal(item);
        }

        public void Insert(int index, T item)
        {
            CC.Validate.ArgumentNull(item, nameof(item));

            inner.Insert(index, item);
            AddInternal(item);
        }

        /// <summary>
        /// Destroyrs the all contained components
        /// </summary>
        public void Clear()
        {
            inner.ForEach(RemoveInternal);
            inner.Clear();
        }

        public bool Contains(T item) => inner.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => inner.CopyTo(array, arrayIndex);

        public bool Remove(T item, bool destroy)
        {
            bool removed = inner.Remove(item);

            if (removed && destroy)
                RemoveInternal(item);

            return removed;
        }

        public bool Remove(T item) => Remove(item, destroy: true);

        public void RemoveAt(int index, bool destroy)
        {
            T toRemove = inner[index];

            if (destroy)
                RemoveInternal(toRemove);

            inner.RemoveAt(index);

        }
        public void RemoveAt(int index) => RemoveAt(index, destroy: true);

        public int IndexOf(T item) => inner.IndexOf(item);

        public void TrimExcess() => inner.TrimExcess();

        public IEnumerator<T> GetEnumerator() => inner.GetEnumerator();

        private void AddInternal(T item)
        {
            item.transform.parent = transform;
        }

        private void RemoveInternal(T item)
        {
            if (DestroyGameObject)
                Destroy(item.gameObject);
            else
                Destroy(item);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
