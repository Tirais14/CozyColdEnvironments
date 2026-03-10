using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.IO.Compression
{
    public class GZipHelper
    {
        public static bool IsCompressed(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length < 2)
                return false;

            return bytes[0] == 0x1F && bytes[1] == 0x8B; //Magic bytes from GZip
        }

        public static bool IsCompressed(ReadOnlySpan<char> text, Encoding? encoding = null)
        {
            if (text.IsEmpty)
                return false;

            encoding ??= Encoding.UTF8;

            Span<byte> buffer = stackalloc byte[2];

            var byteCount = encoding.GetBytes(text, buffer);

            if (byteCount < 2)
                return false;

            return IsCompressed(buffer);
        }

        public static bool IsCompressed(Stream stream)
        {
            Guard.IsNotNull(stream, nameof(stream));

            Span<byte> buffer = stackalloc byte[2];

            var byteCount = stream.Read(buffer);

            if (byteCount < 2)
                return false;

            return IsCompressed(buffer);
        }

        public static bool TryDecompress(
            Stream stream,
            [NotNullWhen(true)] out GZipStream? decompressedStream
            )
        {
            Guard.IsNotNull(stream, nameof(stream));

            if (!IsCompressed(stream))
            {
                decompressedStream = null;
                return false;
            }

            decompressedStream = new GZipStream(stream, CompressionMode.Decompress);
            return true;
        }

        public static async ValueTask<string> TryDecompressAsync(
            string? text,
            Encoding? encoding = null,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (text.IsNullOrWhiteSpace())
                return string.Empty;

            if (!IsCompressed(text))
                return text;

            encoding ??= Encoding.UTF8;

            var byteCount = encoding.GetByteCount(text);

            using var buffer = new PooledArray<byte>(byteCount);

            byteCount = encoding.GetBytes(text, buffer.AsSpan());

            var memStream = new MemoryStream();

#if !PLATFORM_WEBGL
            await memStream.WriteAsync(buffer, cancellationToken);
#else
            memStream.Write(buffer);
#endif

            using var gZipStream = new GZipStream(memStream, CompressionMode.Decompress);

            using var streamReader = new StreamReader(gZipStream);

            return streamReader.ReadToEnd();
        }
    }
}
