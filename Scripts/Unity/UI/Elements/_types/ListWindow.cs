using CCEnvs.Linq;
using CCEnvs.Reflection;
using CCEnvs.Unity.Extensions;
using ZLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using System.Linq;

#nullable enable
namespace CCEnvs.Unity.UI.Elements
{
    /// <summary>
    /// Ignores real indices of components.
    /// Adapted to use with <see cref="Component"/> in <see cref="{T}"/>.
    /// Do not attach any <see cref="Component"/> to the parent <see cref="GameObject"/> indirectly
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListWindow<T> : Window, IList<T>, ITrimmable
    {
        private readonly List<T> inner = new();
        private readonly List<GameObject> gameObjects = new();

        [field: SerializeField]
        public UnityEvent<T> OnAdd { get; } = new();

        [field: SerializeField]
        public UnityEvent<T> OnRemove { get; } = new();

        public IEnumerable<GameObject> GameObjects {
            get => gameObjects.AsEnumerable();
        }
        public IEnumerable<Component> ItemsAsComponents {
            get => inner.AsValueEnumerable().Select(x => x.As<Component>()).AsEnumerable();
        }
        public int Count => inner.Count;
        public T this[int index] {
            get => inner[index];
            set => inner[index] = value;
        }

        protected override bool OpenOnStart => true;

        bool ICollection<T>.IsReadOnly => false;

        protected override void OnStart()
        {
            base.OnStart();
            this.GetAssignedObjectsInChildren<T>().ForEach(x => Add(x));
        }

        private static T[] GetComponentsFrom(GameObject gameObject)
        {
            CC.Validate.ArgumentNull(gameObject, nameof(gameObject));

            if (!gameObject.TryGetAssignedObjects<T>(out var results))
                throw new CCEnvs.Diagnostics.CCException($"Added item must contain on GameObject at least one component with type: {typeof(T).GetName()}.");

            return results;
        }

        public void Add(T item)
        {
            CC.Validate.ArgumentNull(item, nameof(item));

            inner.Add(item);
            AddInternal(item);
        }
        public void Add(GameObject gameObject)
        {
            GetComponentsFrom(gameObject).ForEach(Add);
        }

        public void Insert(int index, T item)
        {
            CC.Validate.ArgumentNull(item, nameof(item));

            inner.Insert(index, item);
            AddInternal(item);
        }
        public void Insert(int index, GameObject gameObject)
        {
            GetComponentsFrom(gameObject).ForEach((x) => Insert(index, x));
        }

        public void Clear(bool destroy)
        {
            gameObjects.ForEach(x => Remove(x, destroy));
        }
        /// <summary>
        /// Destroyrs the all contained components
        /// </summary>
        public void Clear() => Clear(destroy: true);

        public bool Contains(T item) => inner.Contains(item);
        public bool Contains(GameObject gameObject) => gameObjects.Contains(gameObject);

        public void CopyTo(T[] array, int arrayIndex) => inner.CopyTo(array, arrayIndex);
        public void CopyTo(GameObject[] array, int arrayIndex)
        {
            gameObjects.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item, bool destroy)
        {
            bool removed = inner.Remove(item);

            if (removed && destroy)
                RemoveInternal(item);

            return removed;
        }
        /// <summary>
        /// Also destroys parent <see cref="GameObject"/>
        /// </summary>
        public bool Remove(T item) => Remove(item, destroy: true);
        public bool Remove(GameObject gameObject, bool destroy)
        {
            if (!gameObjects.Contains(gameObject))
                return false;

            GetComponentsFrom(gameObject).ForEach(x => Remove(x, destroy: false));
            return true;
        }
        /// <summary>
        /// Also destroys
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public bool Remove(GameObject gameObject) => Remove(gameObject, destroy: true);

        public void RemoveAt(int index, bool destroy)
        {
            T toRemove = inner[index];

            if (destroy)
                RemoveInternal(toRemove);

            inner.RemoveAt(index);

        }
        public void RemoveAt(int index) => RemoveAt(index, destroy: true);

        public int IndexOf(T item) => inner.IndexOf(item);

        public void TrimExcess()
        {
            inner.TrimExcess();
            gameObjects.TrimExcess();
        }

        public IEnumerator<T> GetEnumerator() => inner.GetEnumerator();

        private void AddInternal(T item)
        {
            var compt = item.As<Component>();
            OnAdd?.Invoke(item);

            compt.transform.parent = transform;
            gameObjects.Add(compt.gameObject);
        }

        private void RemoveInternal(T item)
        {
            var compt = item.As<Component>();
            var go = compt.gameObject;
            OnRemove?.Invoke(item);

            int bindedItemCount = ItemsAsComponents.Count(x => x.gameObject == go);

            Destroy(compt);

            if (bindedItemCount == 1)
                gameObjects.Remove(go);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    /// <summary>
    /// Ignores real indices of components. Adapted to use with <see cref="Component"/> in <see cref="{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListWindow : ListWindow<Component>
    {
    }
}
