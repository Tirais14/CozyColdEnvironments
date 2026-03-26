using CCEnvs.Attributes;
using CCEnvs.Caching;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using Humanizer;
using System;
using System.IO;
using System.Text.RegularExpressions;

#nullable enable
namespace CCEnvs
{
    public static class AssetNameHelper
    {
        public static void Parse(
            string assetName,
            out string name,
            out int id
            )
        {
            if (assetName is null)
                throw new ArgumentNullException(nameof(assetName));

            string[] parts = assetName.Split('_');

            if (parts.Length == 1)
            {
                name = assetName;
                id = default!;
                return;
            }

            id = int.Parse(parts[0]);
            name = parts[1];
        }

        public static void ParseReversed(
            string assetName,
            out string name,
            out int id
            )
        {
            if (assetName is null)
                throw new ArgumentNullException(nameof(assetName));

            string[] parts = assetName.Split('_');

            if (parts.Length == 1)
            {
                name = assetName;
                id = default!;
                return;
            }

            id = int.Parse(parts[^1]);
            name = parts[^2];
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

            if (decloniszedNameCache.TryGetValue(assetName, out var declonizedAssetName))
                return declonizedAssetName;

            var match = Regex.Match(assetName, @"^(\w+)");

            declonizedAssetName = match.Value ?? assetName;

            if (decloniszedNameCache.TryAdd(assetName, declonizedAssetName, out var entry))
                entry.ExpirationTimeRelativeToNow = 10.Minutes();

            return declonizedAssetName;
        }

        public static string? ResolvePrefix(UnityEngine.Object asset)
        {
            Guard.IsNotNull(asset);

            return ResolvePrefix(asset.GetType());
        }

        public static string? ResolvePrefix(Type assetType)
        {
            if (assetType.IsType<UnityEngine.Texture>()
                ||
                assetType.IsType<UnityEngine.Texture2D>())
            {
                return "TX_";
            }
            else if (assetType.IsType<UnityEngine.GameObject>())
                return "PFB_";
            else if (assetType.IsType<UnityEngine.Material>())
                return "MAT_";
            else if (assetType.IsType<UnityEngine.TerrainLayer>())
                return "TER_LR_";
            else if (assetType.IsType<UnityEngine.AudioClip>())
                return "ACL_";
            else if (assetType.IsType<UnityEngine.ScriptableObject>())
                return "CFG_";

            return null;
        }

        public static string AddTypePrefix(Type type, string name)
        {
            if (ResolvePrefix(type).IsNull(out var prefix)
                ||
                !name.StartsWith(prefix))
            {
                return name;
            }

            return $"{prefix}{name}";
        }

        public static string RemoveTypePrefix(Type type, string name)
        {
            if (ResolvePrefix(type).IsNull(out var prefix)
                ||
                !name.StartsWith(prefix))
            {
                return name;
            }

            return name[prefix.Length..];
        }
#endif
    }
}
