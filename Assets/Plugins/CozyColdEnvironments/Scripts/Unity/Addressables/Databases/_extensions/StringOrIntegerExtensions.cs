using CCEnvs.Unity.AddrsAssets.Databases;
using CCEnvs.Unity.EditorSerialization;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public static class StringOrIntegerExtensions
    {
        public static AssetKey ToAssetKey(this StringOrInteger source)
        {
            if (source.IsDefault())
                return default;

            if (source.IsNumber)
                return new AssetKey(source.Number!.Value);

            return new AssetKey(source.ToString());
        }
    }
}
