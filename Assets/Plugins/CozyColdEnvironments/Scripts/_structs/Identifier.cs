using CCEnvs.Pools;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Caching;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs
{
    [Serializable]
    public readonly struct Identifier : IEquatable<Identifier>
    {
        public int? Number { get; }

        public string? Text { get; }

        public Identifier(int? number = null, string? text = null)
        {
            Number = number;
            Text = text;
        }

        public Identifier(Enum @enum)
            :
            this()
        {
            Text = @enum.ToString();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Identifier left, Identifier right)
        {
            return left.Equals(right);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Identifier left, Identifier right)
        {
            return !(left == right);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Identifier((int num, string text) input)
        {
            return new Identifier(input.num, input.text);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Identifier(int number)
        {
            return new Identifier(number);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Identifier(string text)
        {
            return new Identifier(text: text);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Identifier(Enum @enum)
        {
            return new Identifier(@enum);
        }

        public static Identifier Create<T>(T value)
            where T : struct, Enum
        {
            return new Identifier(number: value.GetHashCode(), text: value.ToString());
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Identifier WithText(string? text)
        {
            return new Identifier(Number, text);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Identifier WithNumber(int? number)
        {
            return new Identifier(number ?? default, Text);
        }

        public readonly bool Equals(Identifier other)
        {
            return Number == other.Number
                   &&
                   Text == other.Text;
        }
        public readonly override bool Equals(object obj)
        {
            return obj is Identifier typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Number, Text);
        }

        public readonly override string ToString()
        {
            if (this == default)
                return TypeCache<Identifier>.FullName;

            using var stringBuilder = StringBuilderPool.Shared.Get();

            stringBuilder.Value.Append(Number.ToString());
            stringBuilder.Value.Append(" - ");
            stringBuilder.Value.Append(Text);

            return stringBuilder.Value.ToString();
        }
    }
}
