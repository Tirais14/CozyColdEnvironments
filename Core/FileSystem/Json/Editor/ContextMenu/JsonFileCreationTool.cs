using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UTIRLib.FileSystem.Json.Editor
{
    public class JsonFileCreationTool : MonoBehaviour
    {
        [MenuItem("Assets/Create/Scripting/Json")]
        public static void CreateJson()
        {
            string projectViewActiveFolder = $"{Application.dataPath}/{GetActiveFolder().Replace("Assets/", string.Empty)}";
            if (!Directory.Exists(projectViewActiveFolder) || !projectViewActiveFolder.Contains("Assets")) { Debug.LogError("Directory: " + projectViewActiveFolder + " doesn't valid!"); return; }

            const string DefaultJsonFileName = "EmptyTextFile.json";
            string fullPathOfNewFile = Path.Combine(projectViewActiveFolder, DefaultJsonFileName);
            if (File.Exists(fullPathOfNewFile)) { Debug.LogError("File with name " + DefaultJsonFileName + " already exists!"); return; }

            File.Create(fullPathOfNewFile).Close();
            AssetDatabase.Refresh();
        }

        public static string GetActiveFolder()
        {
            var method = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object[] args = new object[] { null };
            bool found = (bool)method.Invoke(null, args);
            if (!found) { return string.Empty; }

            string path = (string)args[0];
            return path;
        }
    }
}