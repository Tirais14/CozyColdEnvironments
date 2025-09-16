using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.Files
{
    /// <summary>
    /// Structure for convenient work with file system paths.
    /// Includes overrided operators.
    /// </summary>
    public readonly struct Path : IEquatable<Path>
    {
        public static char[] Separators => new char[] {
            System.IO.Path.DirectorySeparatorChar,
            System.IO.Path.AltDirectorySeparatorChar
        };
        public static char DefaultDirectorySeparator => System.IO.Path.DirectorySeparatorChar;

        public readonly string value;
        public readonly PathStyle style;

        public readonly string FileName => FSPathHelper.GetFilename(value);
        public readonly bool HasFileName => FSPathHelper.GetFilename(value).IsNotNullOrEmpty();
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

        public Path(PathStyle style, params string[] pathParts) : this()
        {
            this.style = style;

            if (pathParts.IsNullOrEmpty())
                value = string.Empty;
            else
                value = FSPathHelper.Combine(style, pathParts);
        }

        public Path(params string[] pathParts) : this(PathStyle.Default, pathParts)
        {
        }

        public Path(Path path) : this(path.style, path.value)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly string[] Split() => FSPathHelper.Split(value);

        /// <summary>
        /// Same as: (<see cref="Path"/> * <see cref="Style"/>)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Path With(PathStyle style)
        {
            return new Path(style, value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Path With(string filename, PathStyle style)
        {
            string filenameChanged = FSPathHelper.SetFilename(value, filename);

            return new Path(style, filenameChanged);
        }

        /// <exception cref="StringArgumentException"></exception>
        public readonly Path WithExtension(string extension)
        {
            if (extension.IsNullOrEmpty())
                throw new StringArgumentException(nameof(extension), extension);

            string changed = System.IO.Path.ChangeExtension(value, extension);

            return new Path(style, changed);
        }

        /// <summary>
        /// Changes filename in path
        /// </summary>
        /// <exception cref="StringArgumentException"></exception>
        public readonly Path WithFileName(string filename)
        {
            if (filename.IsNullOrEmpty())
                throw new StringArgumentException(nameof(filename), filename);

            string changed = FSPathHelper.SetFilename(value, filename);

            return new Path(style, changed);
        }

        public readonly Path WithPathParts(params string[] pathParts)
        {
            return new Path(style, pathParts);
        }
        public readonly Path WithPathParts(IEnumerable<string> pathParts)
        {
            return WithPathParts(pathParts.ToArray());
        }
        public readonly Path WithPathParts(IEnumerable<Path> pathParts)
        {
            return new Path(style, pathParts.ToStringArray());
        }
        public readonly Path WithPathParts(params Path[] pathParts)
        {
            return WithPathParts((IEnumerable<Path>)pathParts);
        }

        public readonly override int GetHashCode() => HashCode.Combine(value, style);

        public readonly bool Equals(Path other)
        {
            return value == other.value
                   &&
                   style == other.style;
        }
        public readonly override bool Equals(object obj)
        {
            return obj is Path path && Equals(path);
        }

        public readonly override string ToString() => value ?? string.Empty;

        public static Path operator +(Path a, string b)
        {
            return new Path(FSPathHelper.Combine(a.value, b));
        }
        public static Path operator +(Path a, IEnumerable<string> b)
        {
            return new Path(FSPathHelper.Combine(a.value, b));
        }
        public static Path operator +(Path a, Path b)
        {
            return new Path(FSPathHelper.Combine(a.value, b));
        }
        public static Path operator +(Path a, IEnumerable<Path> b)
        {
            return new Path(FSPathHelper.Combine(a.value, b.ToStringArray()));
        }

        public static Path operator -(Path a, string b)
        {
            return FSPathHelper.RemoveLast(a, b);
        }
        public static Path operator -(Path a, Path b)
        {
            return FSPathHelper.RemoveLast(a, b.value);
        }

        public static Path operator *(Path a, PathStyle style)
        {
            return FSPathHelper.SetStyle(a, style);
        }

        public static bool operator ==(Path a, Path b) => a.Equals(b);

        public static bool operator !=(Path a, Path b) => !a.Equals(b);


        public static implicit operator string(Path fileSystemPath)
        {
            return fileSystemPath.value;
        }

        public static explicit operator FileInfo(Path fileSystemPath)
        {
            string normalized = FSPathHelper.Normalize(fileSystemPath.value);

            return new FileInfo(normalized);
        }

        public static explicit operator DirectoryInfo(Path fileSystemPath)
        {
            string normalized = FSPathHelper.Normalize(fileSystemPath.value);

            return new DirectoryInfo(normalized);
        }
    }

    public static class FSPathExtensions
    {
        public static string[] ToStringArray(this IEnumerable<Path> paths)
        {
            return paths.Select(x => x.ToString()).ToArray();
        }
    }
}
