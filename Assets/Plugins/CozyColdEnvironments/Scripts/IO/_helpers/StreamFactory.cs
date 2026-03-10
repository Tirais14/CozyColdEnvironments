using CCEnvs.Pools;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.IO
{
    public static class StreamFactory
    {
        public static MemoryStream CreateMemoryStream(
            ReadOnlySpan<char> text,
            Encoding? encoding = null
            )
        {
            if (text.IsEmpty)
                return new MemoryStream();

            encoding ??= Encoding.UTF8;

            var byteCount = encoding.GetByteCount(text);

            using var buffer = new PooledArray<byte>(byteCount);

            var memStream = new MemoryStream();

            memStream.Write(buffer);

            return memStream;
        }

        public static async ValueTask<MemoryStream> CreateMemoryStreamAsync(
            ReadOnlyMemory<char> text,
            Encoding? encoding = null,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (text.IsEmpty)
                return new MemoryStream();

            encoding ??= Encoding.UTF8;

            var byteCount = encoding.GetByteCount(text.Span);

            using var buffer = new PooledArray<byte>(byteCount);

            var memStream = new MemoryStream();

#if !PLATFORM_WEBGL
            await memStream.WriteAsync(buffer, cancellationToken);
#else
            memStream.Write(buffer);
#endif

            return memStream;
        }
    }
}
