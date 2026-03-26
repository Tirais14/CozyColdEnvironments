#if UNITY_EDITOR
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Editr
{
    public static class EditorLibrary
    {
        public static string DataPath { get; } = Path.Combine(Path.Combine(Application.dataPath, "Editor"));

        public static DirectoryInfo DataDirectory { get; } = new DirectoryInfo(DataPath);

        public static void Save(string path, string serialized)
        {
            Guard.IsNotNullOrWhiteSpace(path);

            if (!DataDirectory.Exists)
                DataDirectory.Create();

            var filePath = System.IO.Path.Combine(DataPath, "CCEnvsData", path);

            var dirName = System.IO.Path.GetDirectoryName(filePath);

            if (!System.IO.Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            try
            {
                File.WriteAllText(filePath, serialized, System.Text.Encoding.UTF8);
            }
            catch (System.Exception ex)
            {
                typeof(EditorLibrary).PrintException(ex);
            }
        }

        public static void Save(string path, object? obj)
        {
            var serialized = JsonConvert.SerializeObject(obj, CC.SerializerSettings);
            Save(path, serialized);
        }

        public static string? Load(string path)
        {
            if (!DataDirectory.Exists)
                return null;

            var filePath = System.IO.Path.Combine(DataPath, "CCEnvsData", path);

            if (!File.Exists(filePath))
                return null;

            try
            {
                return File.ReadAllText(filePath);
            }
            catch (System.Exception ex)
            {
                typeof(EditorLibrary).PrintException(ex);
                return null;
            }
        }

        public static T? Load<T>(string path)
        {
            var serialized = Load(path);

            if (serialized.IsNullOrWhiteSpace())
                return default;

            return JsonConvert.DeserializeObject<T>(serialized, CC.SerializerSettings);
        }
    }
}
#endif