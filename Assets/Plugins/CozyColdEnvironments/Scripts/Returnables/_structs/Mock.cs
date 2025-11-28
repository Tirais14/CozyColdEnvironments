#nullable enable
using System;

namespace CCEnvs.Returnables
{
    public readonly struct Mock : IEquatable<Mock>
    {
        public static Mock Default { get; } = new();

        public static bool operator ==(Mock left, Mock right) => true;

        public static bool operator !=(Mock left, Mock right) => false;

        public bool Equals(Mock other) => this == other;

        public override bool Equals(object obj) => obj is Mock;

        public override int GetHashCode() => 0;

        public override string ToString() => "()";
    }
}
