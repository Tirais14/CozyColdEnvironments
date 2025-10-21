using System;

#nullable enable
namespace CCEnvs
{
    public readonly struct UniID : IEquatable<UniID>
    {
#if UNITY_2017_1_OR_NEWER
        [field: UnityEngine.SerializeField]
#endif
        public int Num0 { get; init; }

#if UNITY_2017_1_OR_NEWER
        [field: UnityEngine.SerializeField]
#endif
        public int Num1 { get; init; }

#if UNITY_2017_1_OR_NEWER
        [field: UnityEngine.SerializeField]
#endif
        public string Str0 { get; init; }

#if UNITY_2017_1_OR_NEWER
        [field: UnityEngine.SerializeField]
#endif
        public string Str1 { get; init; }

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
            return new UniID
            {
                Str0 = value.ToString()
            };
        }
        public static UniID FromEnum(Enum value, Enum value1)
        {
            return new UniID
            {
                Str0 = value.ToString(),
                Str1 = value1.ToString()
            };
        }

        public static UniID FromEnum<T0>(T0 value)
            where T0 : struct, Enum
        {
            return new UniID
            {
                Str0 = value.ToString(),
            };
        }
        public static UniID FromEnum<T0, T1>(T0 value, T1 value1)
            where T0 : struct, Enum
            where T1 : struct, Enum
        {
            return new UniID
            {
                Str0 = value.ToString(),
                Str1 = value1.ToString()
            };
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
