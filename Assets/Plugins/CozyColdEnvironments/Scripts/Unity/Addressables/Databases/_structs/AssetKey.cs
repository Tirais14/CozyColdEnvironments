using CCEnvs.FuncLanguage;
using System;
using System.Diagnostics;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    [DebuggerDisplay("HashCode: {GetHashCode()}; String {ToString()}")]
    public readonly struct AssetKey : IEquatable<AssetKey>
    {
        public Maybe<string> AssetName { get; }
        public int AssetID { get; }

        public AssetKey(string? assetName,
                        int assetID)
        {
            AssetName = assetName;
            AssetID = assetID;
        }

        public AssetKey(string assetName)
            :
            this(assetName, assetID: default)
        {
        }

        public AssetKey(int assetID)
            :
            this(assetName: null, assetID)
        {
        }

        public AssetKey(Object asset)
            :
            this()
        {
            CC.Guard.IsNotNull(asset, nameof(asset));

            AssetName = asset.name;

            if (asset is IIDMarked idMarked)
                AssetID = idMarked.ID.AsOrDefault<int>().GetValue();
        }

        public static bool operator ==(AssetKey left, AssetKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AssetKey left, AssetKey right)
        {
            return !(left == right);
        }

        public AssetKey With(string? assetName)
        {
            return new AssetKey(assetName, AssetID);
        }
        public AssetKey With(int assetID)
        {
            return new AssetKey(AssetName.GetValue(), assetID);
        }

        public bool Equals(AssetKey other)
        {
            return AssetName == other.AssetName
                   &&
                   AssetID == other.AssetID;
        }
        public override bool Equals(object obj)
        {
            return obj is AssetKey typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AssetName, AssetID);
        }

        public override string ToString()
        {
            return $"{nameof(AssetName)}: {AssetName}; {nameof(AssetID)}: {AssetID};";
        }
    }
}
