using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs
{
    public interface IDatabase : IEnumerable
    {
        int Count

        object GetValue(object key);

        T GetValue<T>();
    }

    public interface IDatabase<TKey, TValue> : IDatabase, IReadOnlyDictionary<TKey, TValue>
    {
        TValue GetValue(TKey key);

        object IDatabase.GetValue(object key)
        {
            CC.Validate.ArgumentNull(key, nameof(key));
            if (key is not TKey typed)
                return CC.Throw.InvalidCast(key.GetType(), typeof(TKey));

            return GetValue(typed)!;
        }
    }
}
