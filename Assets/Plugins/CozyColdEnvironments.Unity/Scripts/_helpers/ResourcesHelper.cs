using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CCEnvs.Unity
{
    public static class ResourcesHelper
    {
        public static bool IsInResourcesDirectory(string path)
        {
            return path.Contains("assets", StringComparison.InvariantCultureIgnoreCase) &&
                 path.Contains("resources", StringComparison.InvariantCultureIgnoreCase);
        }

        public static string GetRelativePath(string path)
        {
            if (!IsInResourcesDirectory(path))
                return path;

            string resourcesDirectory = Regex.Match(path, @"^(.*)(Resources)").Value;

            return Path.GetRelativePath(resourcesDirectory, path);
        }
    }
}