#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UTIRLib.Collections;
using UTIRLib.Diagnostics;
using static UTIRLib.FileSystem.FSPath;

namespace UTIRLib.FileSystem
{
    public static class FSPathHelper
    {
        public static string Normalize(string path)
        {
            return SetStyle(path, PathStyle.Default);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static string[] Split(string path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (path.IsNullOrEmpty())
                return Array.Empty<string>();

            path = Normalize(path);

            if (Path.IsPathRooted(path) && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string root = Path.GetPathRoot(path)!;

                if (root.Length > 0)
                {
                    string remainingPath;
                    string[] remainingParts;
                    List<string> parts = new() { root.TrimEnd(GetDirectorySeparator()) };

                    remainingPath = path[root.Length..];
                    remainingParts = remainingPath.Split(new[] { GetDirectorySeparator() },
                                                         StringSplitOptions.RemoveEmptyEntries);

                    parts.AddRange(remainingParts);

                    return parts.ToArray();
                }
            }

            // Обычная обработка для Unix-подобных систем и относительных путей
            char[] separators = new[] { GetDirectorySeparator() };
            return path.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <exception cref="InvalidDataException"></exception>
        public static char GetDirectorySeparator(PathStyle style = PathStyle.Default)
        {
            return style switch {
                PathStyle.Default => DefaultDirectorySeparator,
                PathStyle.Windows => '\\',
                PathStyle.Universal => '/',
                _ => throw new InvalidDataException(style.ToString()),
            };
        }

        public static string SetStyle(string path, PathStyle style)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (path.IsEmpty()) 
                return path;

            return style switch {
                PathStyle.Default => path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar),
                PathStyle.Windows => path.Replace('/', '\\'),
                PathStyle.Universal => path.Replace('\\', '/'),
                _ => path,
            };
        }

        public static FSPath SetStyle(FSPath path, PathStyle style)
        {
            return new FSPath(style, path.value);
        }

        public static string Combine(PathStyle style, params string[] pathParts)
        {
            if (pathParts is null)
                throw new ArgumentNullException(nameof(pathParts));
            if (pathParts.IsEmpty()) 
                return string.Empty;

            string combined = Path.Combine(pathParts);

            return SetStyle(combined, style);
        }
        public static string Combine(PathStyle style, params FSPath[] pathParts)
        {
            return Combine(style, pathParts.ToStringArray());
        }
        public static string Combine(params string[] pathParts)
        {
            return Combine(PathStyle.Default, pathParts);
        }
        public static string Combine(string path,
                                     IEnumerable<string> pathParts,
                                     PathStyle style = PathStyle.Default)
        {


            string[] parts = new ArrayS<string>(path)
                                 .Concat(pathParts)
                                 .ToArray();

            return Combine(style, parts);
        }
        /// <param name="style">if null, uses path style</param>
        public static string Combine(FSPath path,
                                     IEnumerable<string> pathParts,
                                     PathStyle? style = null)
        {
            return Combine(path.value, pathParts, style ?? path.style);
        }
        public static string Combine(string path,
                                     IEnumerable<FSPath> pathParts,
                                     PathStyle style = PathStyle.Default)
        {


            return Combine(path, pathParts.ToStringArray(), style);
        }
        /// <param name="style">if null, uses path style</param>
        public static string Combine(FSPath path,
                                     IEnumerable<FSPath> pathParts,
                                     PathStyle? style = null)
        {
            return Combine(path.value, pathParts, style ?? path.style);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static string RemoveLast(string path, PathStyle style = PathStyle.Default)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (path.IsEmpty()) 
                return string.Empty;

            string[] parts = Split(path);

            if (parts.IsEmpty())
                return string.Empty;
            else if (parts.Length == 1 && Path.IsPathRooted(path))
                return parts[0];

            return Combine(style, parts[0..^1]);
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="StringArgumentException"></exception>
        public static string RemoveLast(string path,
                                        string toRemove,
                                        PathStyle style = PathStyle.Default)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (path.IsEmpty())
                return string.Empty;
            if (toRemove.IsNullOrEmpty())
                throw new StringArgumentException(nameof(toRemove), toRemove);

            string[] parts = Split(path);
            List<string> proccessed = new();
            bool isPartRemoved = false;
            for (int i = parts.Length - 1; i > -1; i--)
            {
                if (!isPartRemoved && parts[i] == toRemove)
                {
                    isPartRemoved = true;
                    continue;
                }

                proccessed.Add(parts[i]);
            }

            return Combine(style, proccessed.ToArray());
        }
        /// <param name="style">if null, uses path style</param>
        public static FSPath RemoveLast(FSPath path,
                                        string toRemove,
                                        PathStyle? style = null)
        {
            string result = RemoveLast(path.value, toRemove, style ?? path.style);

            return new FSPath(result);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static string GetFilename(string path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (path.IsEmpty())
                return string.Empty;

            return Path.GetFileName(path);
        }

        public static bool TryGetFilename(string path, [NotNullWhen(true)] out string? filename)
        {
            filename = GetFilename(path);

            return filename.IsNotNullOrEmpty();
        }

        /// <exception cref="StringArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static string SetFilename(string path,
                                         string filename,
                                         PathStyle style = PathStyle.Default)
        {
            if (path.IsNullOrEmpty())
                throw new StringArgumentException(nameof(path));
            if (filename is null)
                throw new ArgumentNullException(nameof(filename));

            if (!TryGetFilename(path, out _))
                return Combine(path, filename);

            string pathWithoutName = RemoveLast(path);

            return Combine(pathWithoutName, filename);
        }
    }
}
