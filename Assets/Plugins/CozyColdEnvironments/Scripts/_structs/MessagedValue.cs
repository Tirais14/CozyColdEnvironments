#nullable enable
using System;
using System.Collections.Generic;

namespace CCEnvs
{
    public static class MessagedValue
    {
        public static MessagedValue<T> Create<T>(string message, T assigned)
        {
            return new MessagedValue<T>(message, assigned);
        }

        public static MessagedValue<T> Empty<T>(T assigned)
        {
            return new MessagedValue<T>(string.Empty, assigned);
        }
    }
    public readonly struct MessagedValue<T> : IEquatable<MessagedValue<T>>
    {
        public MessagedValue<T> Default => default;

        public readonly string Message;
        public readonly T Value;

        public MessagedValue(string message, T value)
        {
            Message = message;
            Value = value;
        }

        public static implicit operator T(MessagedValue<T> source)
        {
            return source.Value;
        }

        public static bool operator ==(MessagedValue<T> left, MessagedValue<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MessagedValue<T> left, MessagedValue<T> right)
        {
            return !(left == right);
        }

        public bool Equals(MessagedValue<T> other)
        {
            return Message == other.Message
                   &&
                   EqualityComparer<T>.Default.Equals(Value, other.Value);
        }
        public override bool Equals(object obj)
        {
            return obj is MessagedValue<T> typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Message, Value);
        }

        public override string ToString()
        {
            return $"{nameof(Message)}: {Message} | {nameof(Value)}: {Value}.";
        }
    }
}
