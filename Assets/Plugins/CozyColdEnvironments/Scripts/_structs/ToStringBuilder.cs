using CCEnvs.Pools;
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

        private int fieldCount;

        public ToStringBuilder(StringBuilder? sb)
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
        }

        public static bool operator ==(ToStringBuilder left, ToStringBuilder right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ToStringBuilder left, ToStringBuilder right)
        {
            return !(left == right);
        }

        public ToStringBuilder Add<T>(string? fieldName, T? fieldValue)
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

            fieldCount++;

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
