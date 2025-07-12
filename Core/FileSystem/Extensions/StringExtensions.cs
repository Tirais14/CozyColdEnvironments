using System.IO;
using System.Runtime.CompilerServices;

#nullable enable
namespace UTIRLib.FileSystem
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
        public static FSPath ToFilePath(this string str) => new(str);
    }
}