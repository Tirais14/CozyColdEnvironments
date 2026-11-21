using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Services
{
    public readonly struct ResolvedBinder : IEquatable<ResolvedBinder>
    {
        public IServiceBinder Value { get; }

        public BindingType Type { get; }

        public ResolvedBinder(IServiceBinder value, BindingType type)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            Value = value;
            Type = type;
        }

        public static bool operator ==(ResolvedBinder left, ResolvedBinder right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ResolvedBinder left, ResolvedBinder right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is ResolvedBinder binder && Equals(binder);
        }

        public bool Equals(ResolvedBinder other)
        {
            return EqualityComparer<IServiceBinder>.Default.Equals(Value, other.Value) &&
                   Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Type);
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty("Contract", Value.Contract)
                .AddProperty(nameof(Type), Type)
                .ToStringAndDispose();
        }
    }
}
