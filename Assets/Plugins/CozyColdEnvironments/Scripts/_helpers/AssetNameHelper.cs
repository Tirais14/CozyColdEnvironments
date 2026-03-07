using System;
using System.Text.RegularExpressions;
using CCEnvs.Attributes;
using CCEnvs.Caching;
using Humanizer;

#nullable enable
namespace Game
{
    public static class AssetNameHelper
    {
        public static void Parse(string assetName,
                                 out string name,
                                 out int id)
        {
            if (assetName is null)
                throw new ArgumentNullException(nameof(assetName));

            string[] parts = assetName.Split('-');

            if (parts.Length == 1)
            {
                name = assetName;
                id = default!;
                return;
            }

            id = int.Parse(parts[0]);
            name = parts[1];
        }

#if UNITY_2017_1_OR_NEWER

        [OnInstallResetable]
        private readonly static Cache<string, string> decloniszedNameCache = new()
        {
            ExpirationScanFrequency = 30.Seconds(),
        };

        /// <summary>
        /// Deletes (Clone) string from instantiated <see cref="UnityEngine.GameObject"/>
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static string DeleteCloneSuffix(string assetName)
        {
            if (assetName is null)
                throw new ArgumentNullException(assetName);

            if (decloniszedNameCache.TryGet(assetName, out var declonizedAssetName))
                return declonizedAssetName;

            var match = Regex.Match(assetName, @"^(\w+)");

            declonizedAssetName = match.Value ?? assetName;

            if (decloniszedNameCache.TryAdd(assetName, declonizedAssetName, out var entry))
                entry.ExpirationTimeRelativeToNow = 10.Minutes();

            return declonizedAssetName;
        }
#endif
    }
}
