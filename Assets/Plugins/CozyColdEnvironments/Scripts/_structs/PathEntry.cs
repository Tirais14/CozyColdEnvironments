using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.Files
{
    public partial struct PathEntry
    {
        public static string Combine(PathStyle style, params PathEntry[] pathParts)
        {
            return PathHelper.Combine(style, pathParts.ToStringArray());
        }

        /// <param name="style">if null, uses path style</param>
        public static string Combine(PathEntry path,
                                     IEnumerable<string> pathParts,
                                     PathStyle? style = null)
        {
            return PathHelper.Combine(path.value, pathParts, style ?? path.style);
        }

        public static string Combine(string path,
                                     IEnumerable<PathEntry> pathParts,
                                     PathStyle style = PathStyle.Default)
        {


            return PathHelper.Combine(path, pathParts.ToStringArray(), style);
        }

        /// <param name="style">if null, uses path style</param>
        public static string Combine(PathEntry path,
                                     IEnumerable<PathEntry> pathParts,
                                     PathStyle? style = null)
        {
            return Combine(path.value, pathParts, style ?? path.style);
        }

        public static PathEntry SetStyle(PathEntry path, PathStyle style)
        {
            return new PathEntry(style, path.value);
        }

        public static PathEntry RemoveLast(PathEntry path,
                                string toRemove,
                                PathStyle? style = null)
        {
            string result = PathHelper.RemoveLast(path.value, toRemove, style ?? path.style);

            return new PathEntry(result);
        }
    }
    /// <summary>
    /// Structure for convenient work with file system paths.
    /// Includes overrided operators.
    /// </summary>
    public readonly partial struct PathEntry : IEquatable<PathEntry>
    {
        public static char[] Separators => new char[] {
            System.IO.Path.DirectorySeparatorChar,
            System.IO.Path.AltDirectorySeparatorChar
        };
        public static char DefaultDirectorySeparator => System.IO.Path.DirectorySeparatorChar;

        public readonly string value;
        public readonly PathStyle style;

        public readonly string FileName => PathHelper.GetFilename(value);
        public readonly bool HasFileName => PathHelper.GetFilename(value).IsNotNullOrEmpty();
        public string Extension => System.IO.Path.GetExtension(value);
        public bool HasValue => !string.IsNullOrWhiteSpace(value);
        public bool IsValid {
            get {
                return HasValue
                       &&
                       !value.ContainsAny(System.IO.Path.GetInvalidPathChars())
                       &&
                       !FileName.ContainsAny(System.IO.Path.GetInvalidFileNameChars());
            }
        }

        public PathEntry(PathStyle style, params string[] pathParts) : this()
        {
            this.style = style;

            if (pathParts.IsNullOrEmpty())
                value = string.Empty;
            else
                value = PathHelper.Combine(style, pathParts);
        }

        public PathEntry(params string[] pathParts) : this(PathStyle.Default, pathParts)
        {
        }

        public PathEntry(PathEntry path) : this(path.style, path.value)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly string[] Split() => PathHelper.Split(value);

        /// <summary>
        /// Same as: (<see cref="PathEntry"/> * <see cref="Style"/>)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly PathEntry With(PathStyle style)
        {
            return new PathEntry(style, value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly PathEntry With(string filename, PathStyle style)
        {
            string filenameChanged = PathHelper.SetFilename(value, filename);

            return new PathEntry(style, filenameChanged);
        }

        /// <exception cref="EmptyStringArgumentException"></exception>
        public readonly PathEntry WithExtension(string extension)
        {
            Guard.IsNotEmpty(extension, nameof(extension));
            string changed = System.IO.Path.ChangeExtension(value, extension);

            return new PathEntry(style, changed);
        }

        /// <summary>
        /// Changes filename in path
        /// </summary>
        /// <exception cref="EmptyStringArgumentException"></exception>
        public readonly PathEntry WithFileName(string filename)
        {
            Guard.IsNotEmpty(filename, nameof(filename));

            string changed = PathHelper.SetFilename(value, filename);

            return new PathEntry(style, changed);
        }

        public readonly PathEntry WithPathParts(params string[] pathParts)
        {
            return new PathEntry(style, pathParts);
        }
        public readonly PathEntry WithPathParts(IEnumerable<string> pathParts)
        {
            return WithPathParts(pathParts.ToArray());
        }
        public readonly PathEntry WithPathParts(IEnumerable<PathEntry> pathParts)
        {
            return new PathEntry(style, pathParts.ToStringArray());
        }
        public readonly PathEntry WithPathParts(params PathEntry[] pathParts)
        {
            return WithPathParts((IEnumerable<PathEntry>)pathParts);
        }

        public readonly override int GetHashCode() => HashCode.Combine(value, style);

        public readonly bool Equals(PathEntry other)
        {
            return value == other.value
                   &&
                   style == other.style;
        }
        public readonly override bool Equals(object obj)
        {
            return obj is PathEntry path && Equals(path);
        }

        public readonly override string ToString() => value ?? string.Empty;

        public static PathEntry operator +(PathEntry a, string b)
        {
            return new PathEntry(PathHelper.Combine(a.value, b));
        }
        public static PathEntry operator +(PathEntry a, IEnumerable<string> b)
        {
            return new PathEntry(PathHelper.Combine(a.value, b));
        }
        public static PathEntry operator +(PathEntry a, PathEntry b)
        {
            return new PathEntry(PathHelper.Combine(a.value, b));
        }
        public static PathEntry operator +(PathEntry a, IEnumerable<PathEntry> b)
        {
            return new PathEntry(PathHelper.Combine(a.value, b.ToStringArray()));
        }

        public static PathEntry operator -(PathEntry a, string b)
        {
            return RemoveLast(a, b);
        }
        public static PathEntry operator -(PathEntry a, PathEntry b)
        {
            return RemoveLast(a, b.value);
        }

        public static PathEntry operator *(PathEntry a, PathStyle style)
        {
            return SetStyle(a, style);
        }

        public static bool operator ==(PathEntry a, PathEntry b) => a.Equals(b);

        public static bool operator !=(PathEntry a, PathEntry b) => !a.Equals(b);


        public static implicit operator string(PathEntry fileSystemPath)
        {
            return fileSystemPath.value;
        }

        public static explicit operator FileInfo(PathEntry fileSystemPath)
        {
            string normalized = PathHelper.Normalize(fileSystemPath.value);

            return new FileInfo(normalized);
        }

        public static explicit operator DirectoryInfo(PathEntry fileSystemPath)
        {
            string normalized = PathHelper.Normalize(fileSystemPath.value);

            return new DirectoryInfo(normalized);
        }
    }

    public static class FSPathExtensions
    {
        public static string[] ToStringArray(this IEnumerable<PathEntry> paths)
        {
            return paths.Select(x => x.ToString()).ToArray();
        }
    }
}
