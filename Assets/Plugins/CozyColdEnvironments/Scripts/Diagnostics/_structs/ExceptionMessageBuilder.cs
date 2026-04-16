using CCEnvs.Pools;
using CCEnvs.Reflection.Caching;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace CCEnvs.Diagnostics
{
    public struct ExceptionMessageBuilder : IDisposable, IEquatable<ExceptionMessageBuilder>
    {
        private readonly PooledObject<StringBuilder> stringBuilderHandle;

        private readonly StringBuilder stringBuilder;

        private TokenType tokenType;

        public ExceptionMessageBuilder(StringBuilder? stringBuilder)
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
        }

        public readonly ExceptionMessageBuilder Create()
        {
            return new ExceptionMessageBuilder(stringBuilder);
        }

        public static bool operator ==(ExceptionMessageBuilder left, ExceptionMessageBuilder right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ExceptionMessageBuilder left, ExceptionMessageBuilder right)
        {
            return !(left == right);
        }

        public ExceptionMessageBuilder AddMessage(string? msg = null)
        {
            if (msg.IsNotNullOrEmpty())
            {
                ClosePreviousToken();

                tokenType = TokenType.Message;

                stringBuilder.Append(msg);
            }

            return this;
        }

        public ExceptionMessageBuilder AddProperty<T>(string? propName, T? value)
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

        public readonly void Dispose() => stringBuilderHandle.Dispose();

        public readonly override bool Equals(object? obj)
        {
            return obj is ExceptionMessageBuilder builder && Equals(builder);
        }

        public readonly bool Equals(ExceptionMessageBuilder other)
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
                return TypeCache<ExceptionMessageBuilder>.FullName;

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
            var msg = tokenType switch
            {
                TokenType.Message => ". ",
                TokenType.Property => "; ",
                _ => string.Empty
            };

            stringBuilder.Append(msg);
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
