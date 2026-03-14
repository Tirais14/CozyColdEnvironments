#nullable enable
using CCEnvs;
using CCEnvs.Caching;
using CCEnvs.Saves;
using CommunityToolkit.Diagnostics;
using Humanizer;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Core
{
    public static class Saves
    {
        public const string SETTINGS_ARCHIVE_NAME = "Settings";
        public const string SETTINGS_CATALOG_PATH = "Common";
        public const string SETTINGS_GROUP_NAME = "main";

        public const string SAVE_ARCHIVE_NAME = "Default";
        public const string PLAYER_CATALOG_PATH = "Player";
        public const string PLAYER_GROUP_NAME = "player";

        public const long VERSION = 0L;

        private readonly static Cache<string, string> archivePaths = new()
        {
            ExpirationScanFrequency = 30.Seconds()
        };

        public static CreateGroupParameters DefaultCreateGroupParams { get; } = new(string.Empty);

        public static SerializeToFileParameters DefaultSerializeToFileParams { get; } = new()
        {
            Compressed = false
        };

        public static string GetArchivePath(string saveName)
        {
            Guard.IsNotNull(saveName, nameof(saveName));

            return archivePaths.GetOrCreateValue(
                saveName,
                saveName,
                static (entry, saveName) =>
                {
                    entry.ExpirationTimeRelativeToNow = 10.Minutes();

                    var appPath = Application.dataPath; 

                    var path = appPath.Split('/', '\\')[..^1]
                        .Append(Path.Join("Saves", saveName))
                        .JoinStrings(Path.DirectorySeparatorChar);

                    return path;    
                });
        }

        public static SaveGroupIncremental GetPlayerDataGroup()
        {
            return SaveSystem.Archives[GetArchivePath(SAVE_ARCHIVE_NAME)][PLAYER_CATALOG_PATH][PLAYER_GROUP_NAME, true];
        }
    }
}
