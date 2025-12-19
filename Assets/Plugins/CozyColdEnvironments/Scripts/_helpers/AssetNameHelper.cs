using CCEnvs;
using System;

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
        /// <summary>
        /// Deletes (Clone) string from instantiated <see cref="UnityEngine.GameObject"/>
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static string DeleteClonePrefix(string assetName)
        {
            if (assetName is null)
                throw new ArgumentNullException(assetName);

            return assetName.Delete("Clone").TrimEnd(' ');
        }
#endif
    }
}
