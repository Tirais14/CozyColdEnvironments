#nullable enable
using System;

namespace CCEnvs.Reflection.Data
{
    public sealed class CCParameterInfo : IEquatable<CCParameterInfo>
    {
        public Type ParameterType { get; }
        public bool HasDefaultValue { get; }
        public bool HasModifier { get; }

        public CCParameterInfo(Type type,
                             bool hasDefaultValue = false,
                             bool hasModifier = false)
        {
            ParameterType = type;
            HasDefaultValue = hasDefaultValue;
            HasModifier = hasModifier;
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
