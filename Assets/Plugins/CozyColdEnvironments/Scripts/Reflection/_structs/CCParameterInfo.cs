#nullable enable
using System;

namespace CCEnvs.Reflection.Data
{
    public sealed class CCParameterInfo : IEquatable<CCParameterInfo>
    {
        public Type ParameterType { get; }
        public bool HasDefaultValue { get; }
        public Type[] ModifierTypes { get; }
        public bool HasModifier => ModifierTypes is not null;

        public CCParameterInfo(Type type,
                               bool hasDefaultValue = false,
                               Type[]? modifierTypes = null)
        {
            ParameterType = type;
            HasDefaultValue = hasDefaultValue;
            ModifierTypes = modifierTypes ?? Type.EmptyTypes;
        }

        public static CCParameterInfo T<T>()
        {
            return new CCParameterInfo(typeof(T));
        }

        public static bool operator ==(CCParameterInfo? left, CCParameterInfo? right)
        {
            return ReferenceEquals(left, right)
                   ||
                   (left is not null && left.Equals(right));
        }

        public static bool operator !=(CCParameterInfo? left, CCParameterInfo? right)
        {
            return !(left == right);
        }

        public bool Equals(CCParameterInfo? other)
        {
            if (other is null)
                return false;

            return ParameterType == other.ParameterType
                   &&
                   HasDefaultValue == other.HasDefaultValue
                   &&
                   HasModifier == other.HasModifier;
        }

        public override bool Equals(object obj)
        {
            return obj is CCParameterInfo typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ParameterType, HasDefaultValue, HasModifier);
        }
    }
}
