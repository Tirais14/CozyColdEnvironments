using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Collections
{
    public interface IDatabase 
        : IEnumerable,
        ITrimmable,
        ICapacityGetter,
        ICapacitySetter,
        IDisposable
    {
        object GetValue(object key);

        T GetValue<T>(object key);
    }

    public interface IDatabase<TKey, TValue> 
        : IDatabase,
        IReadOnlyDictionary<TKey, TValue>
    {
        TValue GetValue(TKey key);

        T GetValue<T>(TKey key);

        object IDatabase.GetValue(object key)
        {
            CC.Guard.IsNotNull(key, nameof(key));
            if (key is not TKey typed)
                return CC.Throw.InvalidCast(key.GetType(), typeof(TKey));

            return GetValue(typed)!;
        }

        T IDatabase.GetValue<T>(object key)
        {
            return this.To<IDatabase>()
                       .GetValue(key)
                       .To<T>();
        }
    }
}
