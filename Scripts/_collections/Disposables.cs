using CCEnvs.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable

namespace CCEnvs.Disposables
{
    public sealed class Disposables : IDisposableCollection
    {
        private readonly List<IDisposable> disposables;

        public int Count => disposables.Count;

        bool ICollection<IDisposable>.IsReadOnly => false;

        public Disposables() => disposables = new List<IDisposable>();

        public Disposables(int capacity)
        {
            disposables = new List<IDisposable>(capacity);
        }

        public Disposables(IEnumerable<IDisposable> collection)
        {
            disposables = new List<IDisposable>(collection);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void Add(IDisposable item)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));

            disposables.Add(item);
        }

        /// <summary>
        /// Only clear the list.
        /// </summary>
        public void Clear()
        {
            disposables.Clear();
        }

        public bool Contains(IDisposable? item)
        {
            if (item.IsNull())
                return false;

            return disposables.Contains(item);
        }

        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            disposables.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IDisposable> GetEnumerator()
        {
            return disposables.GetEnumerator();
        }

        public bool Remove(IDisposable? item)
        {
            if (item.IsNull())
                return false;

            return disposables.Remove(item);
        }

        public void TrimExcess() => disposables.TrimExcess();

        /// <summary>
        /// Iterates through disposables and call them <see cref="IDisposable.Dispose"/>, after clears the list. Conatiner continue working after disposing.
        /// </summary>
        public void Dispose()
        {
            int disposablesCount = disposables.Count;
            for (int i = 0; i < disposablesCount; i++)
                disposables[i].Dispose();

            Clear();

            GC.SuppressFinalize(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}