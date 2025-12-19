#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using CommunityToolkit.Diagnostics;
using SuperLinq;

namespace CCEnvs.Files
{
    public static class PathHelper
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

            if (System.IO.Path.IsPathRooted(path) && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string root = System.IO.Path.GetPathRoot(path)!;

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

        /// <exception cref="IncorrectDataException"></exception>
        public static char GetDirectorySeparator(PathStyle style = PathStyle.Default)
        {
            return style switch {
                PathStyle.Default => PathEntry.DefaultDirectorySeparator,
                PathStyle.Windows => '\\',
                PathStyle.Universal => '/',
                _ => throw CC.ThrowHelper.InvalidOperationException(style)
            };
        }

        public static string SetStyle(string path, PathStyle style)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (path.IsEmpty()) 
                return path;

            return style switch {
                PathStyle.Default => path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar),
                PathStyle.Windows => path.Replace('/', '\\'),
                PathStyle.Universal => path.Replace('\\', '/'),
                _ => path,
            };
        }

        public static string Combine(PathStyle style, params string[] pathParts)
        {
            if (pathParts is null)
                throw new ArgumentNullException(nameof(pathParts));
            if (pathParts.IsEmpty()) 
                return string.Empty;
            if (pathParts.Length == 1)
                return SetStyle(pathParts[0], style);

            char[] directorySeparartors = Enumerable.Empty<char>()
                .Append(GetDirectorySeparator(PathStyle.Windows))
                .Append(GetDirectorySeparator(PathStyle.Universal))
                .ToArray();

            IEnumerable<IEnumerable<char>> temp = pathParts.SelectMany(part => Split(part))
                .Select(part => (IEnumerable<char>)part)
                .Select(part => part.Where(x => !directorySeparartors.Contains(x)));

            char directorySeparator = GetDirectorySeparator(style);

            StringBuilder sb = temp.Aggregate(new StringBuilder(), (builder, x) =>
            {
                builder.Append(x.ToArray());
                builder.Append(directorySeparator);

                return builder;
            });

            return SetStyle(sb.ToString().TrimEnd(directorySeparator), style);
        }

        public static string Combine(params string[] pathParts)
        {
            return Combine(PathStyle.Default, pathParts);
        }
        public static string Combine(string path,
                                     IEnumerable<string> pathParts,
                                     PathStyle style = PathStyle.Default)
        {


            string[] parts = Range.From(path)
                                  .Concat(pathParts)
                                  .ToArray();

            return Combine(style, parts);
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
            else if (parts.Length == 1 && System.IO.Path.IsPathRooted(path))
                return parts[0];

            return Combine(style, parts[0..^1]);
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="EmptyStringArgumentException"></exception>
        public static string RemoveLast(string path,
                                        string toRemove,
                                        PathStyle style = PathStyle.Default)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (path.IsEmpty())
                return string.Empty;
            Guard.IsNotNullOrEmpty(toRemove, nameof(toRemove));

            string[] parts = Split(path);

            if (parts.IsEmpty())
                return path;

            List<string> proccessed = new(parts.Length - 1);
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

            proccessed.Reverse();
            return Combine(style, proccessed.ToArray());
        }
        /// <param name="style">if null, uses path style</param>


        /// <exception cref="ArgumentNullException"></exception>
        public static string GetFilename(string path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (path.IsEmpty())
                return string.Empty;

            return System.IO.Path.GetFileName(path);
        }

        public static bool TryGetFilename(string path, [NotNullWhen(true)] out string? filename)
        {
            filename = GetFilename(path);

            return filename.IsNotNullOrEmpty();
        }

        /// <exception cref="EmptyStringArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static string SetFilename(string path,
                                         string filename,
                                         PathStyle style = PathStyle.Default)
        {
            Guard.IsNotNullOrEmpty(path, nameof(path));
            if (filename is null)
                throw new ArgumentNullException(nameof(filename));

            if (!TryGetFilename(path, out _))
                return Combine(path, filename);

            string pathWithoutName = RemoveLast(path);

            return Combine(pathWithoutName, filename);
        }
    }
}
