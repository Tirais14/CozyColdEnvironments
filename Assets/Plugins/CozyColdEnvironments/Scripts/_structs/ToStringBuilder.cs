using CCEnvs.Pools;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace CCEnvs
{
    public struct ToStringBuilder : IEquatable<ToStringBuilder>, IDisposable
    {
        private readonly PooledObject<StringBuilder> stringBuilderHandle;

        private readonly StringBuilder stringBuilder;

        private readonly bool indented;

        private int fieldCount;

        public ToStringBuilder(StringBuilder? sb, bool indented = false)
            :
            this()
        {
            if (sb is null)
            {
                stringBuilderHandle = StringBuilderPool.Shared.Get();
                stringBuilder = stringBuilderHandle.Value;
            }
            else
                stringBuilder = new StringBuilder();

            stringBuilder.Append('(');

            this.indented = indented;
        }

        public static ToStringBuilder CreatePooled(bool indented = false)
        {
            return new ToStringBuilder(null, indented);
        }

        public static bool operator ==(ToStringBuilder left, ToStringBuilder right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ToStringBuilder left, ToStringBuilder right)
        {
            return !(left == right);
        }

        public ToStringBuilder AddProperty<T>(string? fieldName, T? fieldValue)
        {
            if (fieldCount > 0)
                stringBuilder.Append(';');

            if (fieldName.IsNotNullOrEmpty())
            {
                stringBuilder.Append(fieldName);
                stringBuilder.Append(": ");
            }

            var fieldValueString = fieldValue.IsNull() ? "null" : fieldValue.ToString();

            stringBuilder.Append(fieldValueString);

            if (indented)
            {
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append('\t');
            }

            fieldCount++;

            return this;
        }

        public ToStringBuilder AddPredicatedProperty<T>(
            bool predicate,
            string? propName,
            T? propValue
            )
        {
            if (!predicate)
                return AddProperty(propName, propValue);

            return this;
        }

        public readonly void Dispose() => stringBuilderHandle.Dispose();

        public readonly ToStringBuilder DisposeQ()
        {
            Dispose();
            return this;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is ToStringBuilder builder && Equals(builder);
        }

        public readonly bool Equals(ToStringBuilder other)
        {
            return stringBuilderHandle.Equals(other.stringBuilderHandle) &&
                   EqualityComparer<StringBuilder>.Default.Equals(stringBuilder, other.stringBuilder) &&
                   fieldCount == other.fieldCount;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(stringBuilderHandle, stringBuilder, fieldCount);
        }

        public readonly override string ToString()
        {
            stringBuilder.Append(')');
            return stringBuilder.ToString();
        }
        public readonly string ToStringAndDispose()
        {
            var str = ToString();
            Dispose();
            return str;
        }
    }
}
