using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

#nullable enable
namespace UTIRLib.FileSystem
{
    public class DirectoryEntry : FileSystemEntry
    {
        public override bool Exists => Directory.Exists(path);
        public DirectoryEntry Parent => new(Directory.GetParent(Path).FullName);
        public DirectoryEntry[] Directories => Directory.GetDirectories(Path)
                                                        .Select(x => new DirectoryEntry(x))
                                                        .ToArray();
        public string[] DirectoryPaths => Directory.GetDirectories(Path);

        public DirectoryEntry(params string[] pathParts) : base(pathParts)
        {
        }

        public DirectoryEntry(FSPath path) : base(path)
        {
        }

        /// <param name="overwrite">if false - throws exception if file exists</param>
        public static DirectoryEntry CreateEntry(bool overwrite, params string[] pathParts)
        {
            DirectoryEntry entry = new(pathParts);
            entry.Create(overwrite);

            return entry;
        }
        /// <param name="overwrite">if false - throws exception if file exists</param>
        public static DirectoryEntry CreateEntry(params string[] pathParts)
        {
            return CreateEntry(overwrite: false, pathParts);
        }
        /// <param name="overwrite">if false - throws exception if file exists</param>
        public static DirectoryEntry CreateEntry(FSPath path, bool overwrite = false)
        {
            return CreateEntry(overwrite, path.value);
        }

        public override bool TrySave(bool overwrite = false)
        {
            if (Exists && !overwrite) return false;

            bool result = TryCreate(overwrite);

            ApplyChanges();

            return result;
        }

        public DirectoryEntry CreateChild(bool overwrite = false, params string[] relativePathParts)
        {
            DirectoryEntry result = new(path.WithPathParts(relativePathParts));
            result.Create(overwrite);

            return result;
        }
        public DirectoryEntry CreateChild(params string[] relativePathParts)
        {
            return CreateChild(overwrite: false, relativePathParts);
        }

        public override bool TryCreate(bool overwrite = false)
        {
            if (Exists && !overwrite)
            {
                return false;
            }
            if (Name.IsNullOrEmpty())
            {
                TirLibDebug.Error(new FileNameException(Name), this);
                return false;
            }

            Directory.CreateDirectory(Path);
            TirLibDebug.Log($"Directory created: \"{Path}\"", this, isExtraInfo: true);
            return true;
        }

        #region GetDirectoryPath
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string? GetDirectoryPath(string byDirectoryNameContains)
        {
            return DirectoryPaths.Single(x => x.Contains(byDirectoryNameContains));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string? GetDirectoryPath(string byDirectoryNameContains,
                                        EnumerationOptions enumerationOptions)
        {
            return GetDirectoryPaths("*", enumerationOptions).
                   Single(x => x.Contains(byDirectoryNameContains));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string? GetDirectoryPath(string byDirectoryNameContains,
                                        SearchOption searchOption)
        {
            return GetDirectoryPaths("*", searchOption).
                   Single(x => x.Contains(byDirectoryNameContains));
        }

        public bool TryGetDirectoryPath(string byDirectoryNameContains,
                                        [NotNullWhen(true)] out string? result)
        {
            result = GetDirectoryPath(byDirectoryNameContains);

            return result.IsNotNullOrEmpty();
        }
        public bool TryGetDirectoryPath(string byDirectoryNameContains,
                                        EnumerationOptions enumerationOptions,
                                        [NotNullWhen(true)] out string? result)
        {
            result = GetDirectoryPath(byDirectoryNameContains, enumerationOptions);

            return result.IsNotNullOrEmpty();
        }
        public bool TryGetDirectoryPath(string byDirectoryNameContains,
                                        SearchOption searchOption,
                                        [NotNullWhen(true)] out string? result)
        {
            result = GetDirectoryPath(byDirectoryNameContains, searchOption);

            return result.IsNotNullOrEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string[] GetDirectoryPaths(string searchPattern)
        {
            return Directory.GetDirectories(Path, searchPattern);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string[] GetDirectoryPaths(string searchPattern, EnumerationOptions enumerationOptions)
        {
            return Directory.GetDirectories(Path, searchPattern, enumerationOptions);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string[] GetDirectoryPaths(string searchPattern, SearchOption searchOption)
        {
            return Directory.GetDirectories(Path, searchPattern, searchOption);
        }
        #endregion GetDirectoryPath

        #region GetDirectory
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DirectoryEntry[] GetDirectories(string searchPattern)
        {
            return GetDirectoryPaths(searchPattern).Select(x => new DirectoryEntry(x)).ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DirectoryEntry[] GetDirectories(string searchPattern, EnumerationOptions enumerationOptions)
        {
            return GetDirectoryPaths(searchPattern, enumerationOptions).Select(x => new DirectoryEntry(x))
                                                                       .ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DirectoryEntry[] GetDirectories(string searchPattern, SearchOption searchOption)
        {
            return GetDirectoryPaths(searchPattern, searchOption).Select(x => new DirectoryEntry(x))
                                                                 .ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DirectoryEntry? FindDirectory(string nameContains)
        {
            return Directories.Single(x => x.Name.Contains(nameContains));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DirectoryEntry? FindDirectory(string nameContains,
                                             EnumerationOptions enumerationOptions)
        {
            return GetDirectories("*", enumerationOptions).
                   Single(x => x.Name.Contains(nameContains));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DirectoryEntry? FindDirectory(string nameContains,
                                             SearchOption searchOption)
        {
            return GetDirectories("*", searchOption).
                   Single(x => x.Name.Contains(nameContains));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindDirectory(string nameContains,
                                     [NotNullWhen(true)] out DirectoryEntry? result)
        {
            result = FindDirectory(nameContains);

            return result is not null;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindDirectory(string nameContains,
                                     EnumerationOptions enumerationOptions,
                                     [NotNullWhen(true)] out DirectoryEntry? result)
        {
            result = FindDirectory(nameContains, enumerationOptions);

            return result is not null;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindDirectory(string nameContains,
                                     SearchOption searchOption,
                                     [NotNullWhen(true)] out DirectoryEntry? result)
        {
            result = FindDirectory(nameContains, searchOption);

            return result is not null;
        }
        #endregion GetDirectory

        public static implicit operator DirectoryInfo(DirectoryEntry directoryEntry)
        {
            return new DirectoryInfo(directoryEntry.Path);
        }

        public static explicit operator FileInfo(DirectoryEntry directoryEntry)
        {
            return new FileInfo(directoryEntry.Path);
        }
    }
}
