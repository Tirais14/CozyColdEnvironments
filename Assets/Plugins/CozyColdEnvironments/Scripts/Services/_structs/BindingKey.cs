using UnityEngine;
using System;
using System.Collections.Generic;

#nullable enable    
namespace CCEnvs.Services
{
    public readonly struct BindingKey : IEquatable<BindingKey>
    {
        public Type Contract { get; }

        public object? ID { get; }

        public BindingKey(Type contract, object? iD)
        {
            Contract = contract;
            ID = iD;
        }

        public static bool operator ==(BindingKey left, BindingKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BindingKey left, BindingKey right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is BindingKey key && Equals(key);
        }

        public bool Equals(BindingKey other)
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
