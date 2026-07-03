using CCEnvs.Pools;
using CCEnvs.Reflection.Caching;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace CCEnvs.Diagnostics
{
    public struct DebugMessageBuilder : IDisposable, IEquatable<DebugMessageBuilder>
    {
        private readonly PooledObject<StringBuilder> stringBuilderHandle;

        private readonly StringBuilder stringBuilder;

        private readonly bool indented;

        private TokenType tokenType;

        public DebugMessageBuilder(StringBuilder? stringBuilder, bool indented = false)
            :
            this()
        {
            if (stringBuilder is null)
            {
                stringBuilderHandle = StringBuilderPool.Shared.Get();
                this.stringBuilder = stringBuilderHandle.Value;
            }
            else
                this.stringBuilder = stringBuilder;

            this.indented = indented;
        }

        public static DebugMessageBuilder CreatePooled(bool indented = false)
        {
            return new DebugMessageBuilder(null, indented);
        }

        public static bool operator ==(DebugMessageBuilder left, DebugMessageBuilder right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DebugMessageBuilder left, DebugMessageBuilder right)
        {
            return !(left == right);
        }

        public DebugMessageBuilder AddMessage(string? msg = null)
        {
            if (msg.IsNotNullOrEmpty())
            {
                ClosePreviousToken();

                tokenType = TokenType.Message;

                stringBuilder.Append(msg);
            }

            return this;
        }

        public DebugMessageBuilder AddProperty<T>(string? propName, T? value)
        {
            bool hasValue = value.IsNotNull();

            if (hasValue)
            {
                ClosePreviousToken();

                tokenType = TokenType.Property;

                WritePropertyStart();

                if (propName.IsNotNullOrEmpty())
                {
                    stringBuilder.Append(propName);
                    stringBuilder.Append(": ");
                }

                stringBuilder.Append(value!.ToString());

                WritePropertyEnd();
            }

            return this;
        }

        public DebugMessageBuilder AddPredicatedProperty<T>(
            bool condition,
            string? propName,
            T value
            )
        {
            if (condition)
                return AddProperty(propName, value);

            return this;
        }

        public readonly void Dispose() => stringBuilderHandle.Dispose();

        public readonly override bool Equals(object? obj)
        {
            return obj is DebugMessageBuilder builder && Equals(builder);
        }

        public readonly bool Equals(DebugMessageBuilder other)
        {
            return stringBuilderHandle.Equals(other.stringBuilderHandle)
                   &&
                   EqualityComparer<StringBuilder>.Default.Equals(stringBuilder, other.stringBuilder)
                   &&
                   tokenType == other.tokenType;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(
                stringBuilderHandle,
                stringBuilder,
                tokenType
                );
        }

        public readonly override string ToString()
        {
            if (stringBuilder is null)
                return TypeCache<DebugMessageBuilder>.FullName;

            return stringBuilder.ToString();
        }

        public readonly string ToStringAndDispose()
        {
            var str = ToString();
            Dispose();
            return str;
        }

        private readonly void ClosePreviousToken()
        {
            var tokenEnd = tokenType switch
            {
                TokenType.Message => ". ",
                TokenType.Property => "; ",
                _ => string.Empty
            };

            stringBuilder.Append(tokenEnd);

            if (indented)
            {
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append('\t');
            }
        }

        private readonly void WritePropertyStart()
        {
            stringBuilder.Append('(');
        }

        private readonly void WritePropertyEnd()
        {
            stringBuilder.Append(')');
        }

        private enum TokenType
        {
            None,
            Message,
            Property
        }
    }
}
