using CCEnvs.Linq;
using CCEnvs.Unity.Extensions;
using SuperLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI.Elements
{
    /// <inheritdoc cref="IListWindow"/>
    /// <summary>
    /// Ignores real indices of components.
    /// Only work with <see cref="Component"/>.
    /// <see cref="{T}"/> maybe interface or another abstraction, but it must be derived from <see cref="Component"/>
    /// </summary>
    public class ListElement<T> : Element, ITrimmable, IListWindow<T>
    {
        private readonly List<(GameObject go, IList<T> values)> inner = new();

        [field: SerializeField]
        public UnityEvent<T> OnAdd { get; } = new();

        [field: SerializeField]
        public UnityEvent<T> OnRemove { get; } = new();

        public IEnumerable<GameObject> GameObjects => inner.Select(item => item.go);
        public IEnumerable<T> Values => inner.SelectMany(item => item.values);
        public bool DestroyGameObjectIfLastComponentRemoved { get; set; } = true;
        public int Count => inner.Count;
        public T[] this[int index] => inner[index].values.ToArray();
        public T this[int index, bool _] => inner[index].values[0];

        protected override bool ShowOnStart => true;

        bool ICollection<(GameObject go, IList<T> values)>.IsReadOnly => false;
        (GameObject go, IList<T> values) IList<(GameObject go, IList<T> values)>.this[int index] {
            get => inner[index];
            set => inner[index] = value; 
        }

        protected override void Start()
        {
            base.Start();

            AddChildrens();
        }

        public void Rebuild()
        {
            Clear();
            AddChildrens();
        }

        public GameObject TakeGameObject(int index)
        {
            GameObject result = inner[index].go;
            result.transform.SetParent(null);
            RemoveAt(index);

            return result;
        }

        /// <summary>
        /// Adds to list and reparent <see cref="GameObject"/>.
        /// If hasn't <see cref="T"/> component it won't be added
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            CC.Guard.NullArgument(item, nameof(item));

            TryAdd(item, out _);
        }
        /// <inheritdoc cref="Add(T)"/>>
        public void Add(GameObject gameObject)
        {
            CC.Guard.NullArgument(gameObject, nameof(gameObject));

            TryAdd(gameObject, out _);
        }

        /// <summary>
        /// Destroys all
        /// </summary>
        public void Clear()
        {
            using var temp = GameObjects.ToArrayPooled();

            temp.Values.ForEach((x) => Remove(x));
        }

        public bool Contains(T item)
        {
            return Values.Contains(item);
        }
        public bool Contains(GameObject gameObject)
        {
            return GameObjects.Contains(gameObject);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Values.CopyTo(array, arrayIndex);
        }
        public void CopyTo(GameObject[] array, int arrayIndex)
        {
            GameObjects.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Destroys <see cref="T"/> or <see cref="GameObject"/>
        /// </summary>
        public bool Remove(T item)
        {
            return TryRemove(item, out _);
        }
        /// <summary>
        /// Destroys <see cref="GameObject"/>
        /// </summary>
        public bool Remove(GameObject gameObject)
        {
            return TryRemove(gameObject, out _);
        }

        public void RemoveAt(int index) => inner.RemoveAt(index);

        public int IndexOf(T item)
        {
            return IndexOf(item.As<Component>().gameObject);
        }
        public int IndexOf(GameObject gameObject)
        {
            CC.Guard.NullArgument(gameObject, nameof(gameObject));

            var found = GameObjects.Index().FirstOrDefault(x => x.Item == gameObject);

            if (found.IsDefault())
                return -1;

            return found.Index;
        }

        public void TrimExcess() => inner.TrimExcess();

        public IEnumerator<(GameObject go, IList<T> values)> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        private bool TryAdd(GameObject gameObject,
            out (GameObject go, IList<T> values) added)
        {
            if (!gameObject.TryGetAssignedObjects<T>(out var results))
            {
                added = default;
                return false;
            }

            added = (gameObject, results);
            inner.Add(added);
            gameObject.transform.SetParent(transform);
            return true;
        }
        private bool TryAdd(T item, out (GameObject go, IList<T> values) added)
        {
            return TryAdd(item.As<Component>().gameObject, out added);
        }

        private bool TryRemove(GameObject gameObject,
            out (GameObject go, IList<T> values) removed)
        {
            removed = inner.FirstOrDefault(item => item.go == gameObject);

            if (removed.IsDefault())
                return false;

            Destroy(gameObject);

            return inner.Remove(removed);
        }
        private bool TryRemove(T value,
           out (GameObject go, IList<T> values) removed)
        {
            var compt = value.As<Component>();
            removed = inner.FirstOrDefault(item => item.go == compt.gameObject);

            if (removed.IsDefault() || inner.Remove(removed))
                return false;

            Destroy(compt);

            if (DestroyGameObjectIfLastComponentRemoved
                &
                !compt.gameObject.TryGetAssignedObjects<T>(out var newValues))
                Remove(compt.gameObject);
            else
                inner[IndexOf(compt.gameObject)] = (compt.gameObject, newValues);

            return true;
        }

        private void AddChildrens()
        {
            gameObject.GetChilds().ForEach(Add);
        }

        int IList<(GameObject go, IList<T> values)>.IndexOf((GameObject go, IList<T> values) item)
        {
            return inner.IndexOf(item);
        }

        void IList<(GameObject go, IList<T> values)>.Insert(int index, (GameObject go, IList<T> values) item)
        {
            inner.Insert(index, item);
        }

        void ICollection<(GameObject go, IList<T> values)>.Add((GameObject go, IList<T> values) item)
        {
            inner.Add(item);
        }

        bool ICollection<(GameObject go, IList<T> values)>.Contains((GameObject go, IList<T> values) item)
        {
            return inner.Contains(item);
        }

        void ICollection<(GameObject go, IList<T> values)>.CopyTo((GameObject go, IList<T> values)[] array, int arrayIndex)
        {
            inner.CopyTo(array, arrayIndex);
        }

        bool ICollection<(GameObject go, IList<T> values)>.Remove((GameObject go, IList<T> values) item)
        {
            return inner.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();
    }
    /// <summary>
    /// Ignores real indices of components. Adapted to use with <see cref="Component"/> in <see cref="{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListElement : ListElement<Component>
    {
    }
}
