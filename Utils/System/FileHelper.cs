using System.IO;

#nullable enable

namespace UTIRLib.Utils
{
    public static class FileHelper
    {
        /// <returns>true if created</returns>
        public static bool WriteAllText(string path, string content, bool overwrite = false)
        {
            if (File.Exists(path) && !overwrite)
            {
                return false;
            }

            File.WriteAllText(path, content);
            return true;
        }

        public static bool WriteAllText(string content, bool overwrite = false, params string[] pathParts) =>
            WriteAllText(Path.Combine(pathParts), content, overwrite);

        public static bool WriteAllText(string content, params string[] pathParts) =>
            WriteAllText(Path.Combine(pathParts), content, overwrite: false);

        public static void WriteAllTextOverwrite(string content, params string[] pathParts) =>
            WriteAllText(Path.Combine(pathParts), content, overwrite: true);
    }
}