using System.IO;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Files
{
    public static class StringExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FileInfo ToFileInfo(this string str) => new(str);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DirectoryInfo ToDirectoryInfo(this string str) => new(str);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FileEntry ToFileEntry(this string str) => new(str);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DirectoryEntry ToDirectoryEntry(this string str) => new(str);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PathEntry ToFilePath(this string str) => new(str);
    }
}