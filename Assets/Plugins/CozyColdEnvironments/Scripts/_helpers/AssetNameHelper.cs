using CCEnvs.Attributes;
using CCEnvs.Caching;
using CCEnvs.Collections;
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
        public static bool TryParseIDName(
            string assetName,
            out string name,
            out int id,
            string separator = "_",
            int idPosition = 0,
            int namePosition = 1
            )
        {
            Guard.IsNotNull(assetName);
            Guard.IsNotNull(separator);

            string[] parts = assetName.Split(separator);

            name = assetName;
            id = default;

            bool isParsed = false;

            if (idPosition < parts.Length)
                isParsed = int.TryParse(parts[idPosition], out id);

            if (namePosition < parts.Length)
            {
                name = parts[namePosition];
                isParsed = true;
            }

            return isParsed;
        }

        public static bool TryParseIDNameEnum<TEnum>(
            string assetName,
            out string name,
            out int id,
            out TEnum enm,
            string separator = "_",
            int idPosition = 0,
            int enumPosition = 1,
            int namePosition = 2
            )
            where TEnum : unmanaged, Enum
        {
            Guard.IsNotNull(assetName);
            Guard.IsNotNull(separator);

            string[] parts = assetName.Split(separator);

            name = assetName;
            id = default;
            enm = default;

            bool isParsed = false;

            if (idPosition < parts.Length)
                if (int.TryParse(parts[idPosition], out id))
                    isParsed = true;

            if (enumPosition < parts.Length)
                if (Enum.TryParse(parts[enumPosition], out enm))
                    isParsed = true;

            if (namePosition < parts.Length)
            {
                name = parts[namePosition];
                isParsed = true;
            }

            return isParsed;
        }

        //public static bool TryParseIDName(
        //    string assetName,
        //    out string name,
        //    out int id,
        //    string separator = "_"
        //    )
        //{
        //    Guard.IsNotNull(assetName);
        //    Guard.IsNotNull(separator);

        //    string[] parts = assetName.Split(separator);

        //    name = assetName;
        //    id = default;

        //    if (parts.Length == 0)
        //        return false;

        //    name = parts[0];

        //    if (parts.Length > 1)
        //        int.TryParse(parts[1], out id);

        //    return true;
        //}

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

        public static string AddTypePrefixToPath(Type type, string path)
        {
            var name = Path.GetFileName(path);

            var changedName = AddTypePrefix(type, name);

            if (name == changedName)
                return path;

            var pathParts = path.Split('/', '\\');

            path = Path.Combine(pathParts[..^1].AppendToArray(changedName));
            return path;
        }

        public static string AddTypePrefix(Type type, string name)
        {
            if (ResolvePrefix(type).IsNull(out var prefix)
                ||
                name.StartsWith(prefix))
            {
                return name;
            }

            return $"{prefix}{name}";
        }

        public static string RemoveTypePrefixFromPath(Type type, string path)
        {
            var name = Path.GetFileName(path);

            var changedName = RemoveTypePrefix(type, name);

            if (name == changedName)
                return path;

            var pathParts = path.Split('/', '\\');

            path = Path.Combine(pathParts[..^1].AppendToArray(changedName));

            return path;
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
