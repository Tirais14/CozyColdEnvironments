using System;
#if UNITY_2017_1_OR_NEWER
using UnityEngine;
#endif

#nullable enable
namespace CCEnv
{
#if UNITY_2017_1_OR_NEWER
    public struct UniID : IEquatable<UniID>
    {
        [field: SerializeField]
        public int Num0 { get; private set; }

        [field: SerializeField]
        public int Num1 { get; private set; }

        [field: SerializeField]
        public string? Str0 { get; private set; }

        [field: SerializeField]
        public string? Str1 { get; private set; }
#else
    public readonly struct CommonID : IEquatable<CommonID>
    {
        public int Num0 { get; }
        public int Num1 { get; }
        public string? Str0 { get; }
        public string? Str1 { get; }
#endif //UNITY_2017_1_OR_NEWER

        public UniID(int num0, int num1, string? str0, string? str1)
        {
            Num0 = num0;
            Num1 = num1;
            Str0 = str0;
            Str1 = str1;
        }

        public UniID(int num0, string? str0) : this()
        {
            this.Num0 = num0;
            this.Str0 = str0;
        }

        public UniID(int num0) : this()
        {
            this.Num0 = num0;
        }

        public UniID(string? str0) : this()
        {
            this.Str0 = str0;
        }

        public UniID(int num0, int num1) : this(num0)
        {
            this.Num1 = num1;
        }

        public UniID(string? str0, string? str1) : this(str0)
        {
            this.Str1 = str1;
        }

        public static bool operator ==(UniID left, UniID right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UniID left, UniID right)
        {
            return !(left == right);
        }

        public static UniID FromEnum(Enum value)
        {
            return new UniID(value.ToString());
        }
        public static UniID FromEnum(Enum value, Enum value1)
        {
            return new UniID(value.ToString(), value1.ToString());
        }

        public static UniID FromEnum<T>(T value)
            where T : struct, Enum
        {
            return new UniID(value.ToString());
        }
        public static UniID FromEnum<T, T1>(T value, T1 value1)
            where T : struct, Enum
            where T1 : struct, Enum
        {
            return new UniID(value.ToString(), value1.ToString());
        }

        public readonly bool Equals(UniID other)
        {
            return Num0 == other.Num0
                   &&
                   Num1 == other.Num1
                   &&
                   Str0 == other.Str0
                   &&
                   Str1 == other.Str1;
        }
        public readonly override bool Equals(object obj)
        {
            return obj is UniID typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Num0, Num1, Str0, Str1);
        }

        public readonly override string ToString()
        {
            return $"{nameof(Num0)}: {Num0}; {nameof(Num1)}: {Num1}; {nameof(Str0)}: {Str0}; {nameof(Str1)}: {Str1}";
        }
    }
}
