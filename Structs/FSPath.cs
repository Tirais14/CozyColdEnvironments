using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.FileSystem
{
    /// <summary>
    /// Structure for convenient work with file system paths.
    /// Includes overrided operators.
    /// </summary>
    public readonly struct FSPath : IEquatable<FSPath>
    {
        public static char[] Separators => new char[] {
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar
        };
        public static char DefaultDirectorySeparator => Path.DirectorySeparatorChar;

        public readonly string value;
        public readonly PathStyle style;

        public readonly string FileName => FSPathHelper.GetFilename(value);
        public readonly bool HasFileName => FSPathHelper.GetFilename(value).IsNotNullOrEmpty();
        public string Extension => Path.GetExtension(value);
        public bool HasValue => !string.IsNullOrWhiteSpace(value);
        public bool IsValid {
            get {
                return HasValue
                       &&
                       !value.ContainsAny(Path.GetInvalidPathChars())
                       &&
                       !FileName.ContainsAny(Path.GetInvalidFileNameChars());
            }
        }

        public FSPath(PathStyle style, params string[] pathParts) : this()
        {
            this.style = style;

            if (pathParts.IsNullOrEmpty())
                value = string.Empty;
            else
                value = FSPathHelper.Combine(style, pathParts);
        }

        public FSPath(params string[] pathParts) : this(PathStyle.Default, pathParts)
        {
        }

        public FSPath(FSPath path) : this(path.style, path.value)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly string[] Split() => FSPathHelper.Split(value);

        /// <summary>
        /// Same as: (<see cref="FSPath"/> * <see cref="Style"/>)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly FSPath With(PathStyle style)
        {
            return new FSPath(style, value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly FSPath With(string filename, PathStyle style)
        {
            string filenameChanged = FSPathHelper.SetFilename(value, filename);

            return new FSPath(style, filenameChanged);
        }

        /// <exception cref="StringArgumentException"></exception>
        public readonly FSPath WithExtension(string extension)
        {
            if (extension.IsNullOrEmpty())
                throw new StringArgumentException(nameof(extension), extension);

            string changed = Path.ChangeExtension(value, extension);

            return new FSPath(style, changed);
        }

        /// <summary>
        /// Changes filename in path
        /// </summary>
        /// <exception cref="StringArgumentException"></exception>
        public readonly FSPath WithFileName(string filename)
        {
            if (filename.IsNullOrEmpty())
                throw new StringArgumentException(nameof(filename), filename);

            string changed = FSPathHelper.SetFilename(value, filename);

            return new FSPath(style, changed);
        }

        public readonly FSPath WithPathParts(params string[] pathParts)
        {
            return new FSPath(style, pathParts);
        }
        public readonly FSPath WithPathParts(IEnumerable<string> pathParts)
        {
            return WithPathParts(pathParts.ToArray());
        }
        public readonly FSPath WithPathParts(IEnumerable<FSPath> pathParts)
        {
            return new FSPath(style, pathParts.ToStringArray());
        }
        public readonly FSPath WithPathParts(params FSPath[] pathParts)
        {
            return WithPathParts((IEnumerable<FSPath>)pathParts);
        }

        public readonly override int GetHashCode() => HashCode.Combine(value, style);

        public readonly bool Equals(FSPath other)
        {
            return value == other.value && style == other.style;
        }
        public readonly override bool Equals(object obj)
        {
            return obj is FSPath path && Equals(path);
        }

        public readonly override string ToString() => value;

        public static FSPath operator +(FSPath a, string b)
        {
            return new FSPath(FSPathHelper.Combine(a.value, b));
        }
        public static FSPath operator +(FSPath a, IEnumerable<string> b)
        {
            return new FSPath(FSPathHelper.Combine(a.value, b));
        }
        public static FSPath operator +(FSPath a, FSPath b)
        {
            return new FSPath(FSPathHelper.Combine(a.value, b));
        }
        public static FSPath operator +(FSPath a, IEnumerable<FSPath> b)
        {
            return new FSPath(FSPathHelper.Combine(a.value, b.ToStringArray()));
        }

        public static FSPath operator -(FSPath a, string b)
        {
            return FSPathHelper.RemoveLast(a, b);
        }
        public static FSPath operator -(FSPath a, FSPath b)
        {
            return FSPathHelper.RemoveLast(a, b.value);
        }

        public static FSPath operator *(FSPath a, PathStyle style)
        {
            return FSPathHelper.SetStyle(a, style);
        }

        public static bool operator ==(FSPath a, FSPath b) => a.Equals(b);

        public static bool operator !=(FSPath a, FSPath b) => !a.Equals(b);


        public static implicit operator string(FSPath fileSystemPath)
        {
            return fileSystemPath.value;
        }

        public static explicit operator FileInfo(FSPath fileSystemPath)
        {
            string normalized = FSPathHelper.Normalize(fileSystemPath.value);

            return new FileInfo(normalized);
        }

        public static explicit operator DirectoryInfo(FSPath fileSystemPath)
        {
            string normalized = FSPathHelper.Normalize(fileSystemPath.value);

            return new DirectoryInfo(normalized);
        }
    }

    public static class FSPathExtensions
    {
        public static string[] ToStringArray(this IEnumerable<FSPath> paths)
        {
            return paths.Select(x => x.ToString()).ToArray();
        }
    }
}
