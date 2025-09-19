using CCEnvs.Linq;
using System;
using System.Collections.ObjectModel;
using System.Linq;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public readonly struct AssetLabels : IEquatable<AssetLabels>
    {
        public ReadOnlyCollection<string> Labels { get; }

        public AssetLabels(params string[] labels)
        {
            Labels = new ReadOnlyCollection<string>(labels);
        }

        public static bool operator ==(AssetLabels left, AssetLabels right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AssetLabels left, AssetLabels right)
        {
            return !(left == right);
        }

        public string[] ToArray() => Labels.ToArray();

        public bool Equals(AssetLabels other)
        {
            return Labels.SequenceEqual(other.Labels);
        }
        public override bool Equals(object obj)
        {
            return obj is AssetLabels typed && Equals(typed);
        }

        public override int GetHashCode() => Labels.ToHashCode();

        public static implicit operator string[](AssetLabels assetLabels)
        {
            return assetLabels.Labels.ToArray();
        }
    }
}
