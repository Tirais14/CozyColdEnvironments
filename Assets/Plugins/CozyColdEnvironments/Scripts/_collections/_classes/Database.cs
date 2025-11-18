using System.Collections.Generic;
using System.Collections.ObjectModel;

#pragma warning disable S3881
namespace CCEnvs.Collections
{
    public class Database<TKey, TValue> 
        : ReadOnlyDictionary<TKey, TValue>,
        IDatabase<TKey, TValue>
    {
        private volatile bool disposeValue;

        public Database(int capacity) : base(new Dictionary<TKey, TValue>(capacity))
        {

        }

        public Database()
            :
            this(capacity: 4)
        {
        }

        public void Add(TKey key, TValue value)
        {
            Dictionary.Add(key, value);
        }

        public bool Remove(TKey key) => Dictionary.Remove(key);

        public TValue GetValue(TKey key) => Dictionary[key];

        public T GetValue<T>(TKey key) => GetValue(key).As<T>();

        public void TrimExcess() => Dictionary.As<Dictionary<TKey, TValue>>().TrimExcess();

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposeValue)
                return;

            if (disposing)
                Do.Nothing();

            disposeValue = true;
        }
    }
}
