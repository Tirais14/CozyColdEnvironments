using System;
using System.Diagnostics;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    [DebuggerDisplay("HashCode: {GetHashCode()}; String {ToString()}")]
    public readonly struct AssetKey : IEquatable<AssetKey>
    {
        public string? AssetName { get; }
        public object? AssetID { get; }

        public AssetKey(string? assetName,
                        object? assetID)
        {
            AssetName = assetName;
            AssetID = assetID;
        }

        public AssetKey(string assetName)
            :
            this(assetName, assetID: null)
        {
        }

        public AssetKey(object assetID)
            :
            this(assetName: null, assetID)
        {
        }

        public AssetKey(Object asset)
            :
            this()
        {
            CC.Validate.ArgumentNull(asset, nameof(asset));

            AssetName = asset.name;

            if (asset is IIDMarked idMarked)
                AssetID = idMarked.ID;
        }

        public static AssetKey ByID(object id)
        {
            CC.Validate.ArgumentNull(id, nameof(id));

            return new AssetKey(id);
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
        public AssetKey With(object? assetID)
        {
            return new AssetKey(AssetName, assetID);
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
            return $"{nameof(AssetName)}: {AssetName} | {nameof(AssetID)}: {AssetID}.";
        }
    }
}
