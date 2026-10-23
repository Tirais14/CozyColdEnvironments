using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Services
{
    public readonly struct FactoryArguments : IReadOnlyList<object>
    {
        public IReadOnlyList<object> Values { get; }

        public int Count => Values?.Count ?? 0;

        public object this[int index] => Values[index];

        public FactoryArguments(IReadOnlyList<object>? values)
        {
            if (values.IsNull())
                Values = Array.Empty<object>();
            else
                Values = values;
        }

        public static implicit operator FactoryArguments(object[] args)
        {
            return new FactoryArguments(args);
        }

        public IEnumerator<object> GetEnumerator() => Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
