using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Services
{
    public readonly struct BindingInfo : IEquatable<BindingInfo>
    {
        public Type Contract { get; }

        public object? ID { get; }

        public BindingInfo(Type contract, object? iD)
        {
            Guard.IsNotNull(contract, nameof(contract));

            Contract = contract;
            ID = iD;
        }

        public static bool operator ==(BindingInfo left, BindingInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BindingInfo left, BindingInfo right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is BindingInfo info && Equals(info);
        }

        public bool Equals(BindingInfo other)
        {
            return EqualityComparer<Type>.Default.Equals(Contract, other.Contract) &&
                   EqualityComparer<object?>.Default.Equals(ID, other.ID);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Contract, ID);
        }
    }
}
