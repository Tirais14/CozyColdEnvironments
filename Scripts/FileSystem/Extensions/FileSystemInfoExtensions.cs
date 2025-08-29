#nullable enable
using System.IO;
using System.Runtime.CompilerServices;

namespace CozyColdEnvironments.FileSystem
{
    public static class FileSystemInfoExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FileEntry ToEntry(this FileInfo fileInfo) => new(fileInfo.FullName);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DirectoryEntry ToEntry(this DirectoryInfo directoryInfo)
        {
            return new(directoryInfo.FullName);
        }
    }
}
