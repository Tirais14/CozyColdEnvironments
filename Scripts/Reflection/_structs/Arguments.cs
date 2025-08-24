using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

#nullable enable
namespace UTIRLib.Reflection.ObjectModel
{
    public readonly struct Arguments
        :
        IReadOnlyList<object?>,
        IEquatable<Arguments>
    {
        private readonly ReadOnlyCollection<object?> values;

        public IReadOnlyList<object?> Values => values;
        public int Count => values.Count;
        public object? this[int index] => values[index];

        public Arguments(params object?[] values)
        {
            this.values = new ReadOnlyCollection<object?>(values);
        }

        public static bool operator ==(Arguments left, Arguments right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Arguments left, Arguments right)
        {
            return !left.Equals(right);
        }

        public static explicit operator object?[](Arguments arguments)
        {
            return arguments.values?.ToArray() ?? Array.Empty<object?>();
        }

        public bool Equals(Arguments other)
        {
            if (values is null && other.values is null)
                return true;
            if (other.values is null)
                return false;

            return values.SequenceEqual(other.values);
        }
        public override bool Equals(object obj)
        {
            return obj is Arguments typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            if (((ICollection<object?>)values).IsNullOrEmpty())
                return 0;

            return values.Aggregate(
                new HashCode(), (hash, x) => { hash.Add(x); return hash; })
                .ToHashCode();
        }

        public override string ToString()
        {
            if (((ICollection<object?>)values).IsNullOrEmpty())
                return "empty";

            var builder = new StringBuilder();
            for (int i = 0; i < values.Count; i++)
                builder.Append($"position = {i}, value = {values[i]?.ToString() ?? string.Empty}; ");

            return builder.ToString();
        }

        public IEnumerator<object?> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
