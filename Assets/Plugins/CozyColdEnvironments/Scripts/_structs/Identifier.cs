using CCEnvs.FuncLanguage;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs
{
    [Serializable]
    public readonly struct Identifier : IEquatable<Identifier>
    {
#if UNITY_2017_1_OR_NEWER
        [field: UnityEngine.SerializeField]
#endif
        public Maybe<int> Number { get; init; }

#if UNITY_2017_1_OR_NEWER
        [field: UnityEngine.SerializeField]
#endif
        public Maybe<string> Text { get; init; }

        public Identifier(Maybe<int> number, string? text)
        {
            Number = number;
            Text = text;
        }

        public Identifier(Maybe<int> number)
            :
            this()
        {
            Number = number;
        }

        public Identifier(string text)
            :
            this()
        {
            Text = text;
        }

        public Identifier(Enum @enum)
            :
            this()
        {
            Text = @enum.ToString();
        }

        public static bool operator ==(Identifier left, Identifier right)
        {
            return left.Equals(right);
        }

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
            return new Identifier(text);
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
            return new Identifier(value.ToString());
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Identifier WithText(string? text)
        {
            return new Identifier(Number.Raw, text);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Identifier WithNumber(Maybe<int> number)
        {
            return new Identifier(number, Text.Raw);
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
            return $"{nameof(Number)}: {Number}; {nameof(Text)}: {Text};";
        }
    }
}
