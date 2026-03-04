using System.IO;
using UnityEngine;

#nullable enable
namespace Core
{
    public static class G
    {
        public static string SavesDirectory { get; } = Path.Combine(Application.dataPath, "Saves");

        public static string SaveDefaultArchivePath { get; } = Path.Combine(SavesDirectory, "Save1");

        public static string SaveDefaultCatalogPath { get; } = "Default";

        public static string SaveDefaultGroupName { get; } = "default";
    }
}
