//using System;
//using System.Collections.Generic;
//using UnityEngine;

//#nullable enable
//namespace CCEnvs.Unity.D3
//{
//    public readonly struct ColliderPackedInfo : IEquatable<ColliderPackedInfo>
//    {
//        public IReadOnlyList<Vector3> Positions { get; }

//        public ColliderPackedInfo(IReadOnlyList<Vector3> positions
//            )
//        {
//            CC.Guard.IsNotNull(container, nameof(container));
//            CC.Guard.IsNotNull(entry, nameof(entry));
//            CC.Guard.IsNotNull(positions, nameof(positions));

//            Container = container;
//            Entry = entry;
//            Positions = positions;
//        }

//        public static bool operator ==(ColliderPackedInfo left, ColliderPackedInfo right)
//        {
//            return left.Equals(right);
//        }

//        public static bool operator !=(ColliderPackedInfo left, ColliderPackedInfo right)
//        {
//            return !(left == right);
//        }

//        public override bool Equals(object? obj)
//        {
//            return obj is ColliderPackedInfo info && Equals(info);
//        }

//        public bool Equals(ColliderPackedInfo other)
//        {
//            return EqualityComparer<Collider>.Default.Equals(Container, other.Container)
//                   &&
//                   EqualityComparer<Collider>.Default.Equals(Entry, other.Entry)
//                   &&
//                   Positions.Equals(other.Positions);
//        }

//        public override int GetHashCode()
//        {
//            return HashCode.Combine(Container, Entry, Position);
//        }
//    }
//}
