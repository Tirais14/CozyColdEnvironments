using CCEnvs.Diagnostics;
using CCEnvs.IO.Compression;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Saves
{
    public static class SaveLoad
    {
        public static async ValueTask<string> FromFileAsync(
            string filePath,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            Guard.IsNotNullOrWhiteSpace(filePath, nameof(filePath));

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToThreadPool();
#endif

            var file = new FileInfo(filePath);

            if (!file.Exists)
            {
                if (CCDebug.Instance.IsEnabled)
                    typeof(SaveLoad).PrintLog($"File: {filePath} not found");

                return string.Empty;
            }

            try
            {
                await SaveSystem.IOSemaphore.WaitAsync(cancellationToken);

                using var fileStream = file.OpenRead();

                if (fileStream.Length == 0)
                    return string.Empty;

                var decompressed = await TryDecompressAsync(fileStream);

                return decompressed;
            }
            catch (Exception ex)
            {
                typeof(SaveLoad).PrintException(ex);

                return string.Empty;
            }
            finally
            {
                SaveSystem.IOSemaphore.Release();

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
                await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif
            }
        }

        public static async ValueTask<SaveData?> DataFromFileAsync(
            string filePath,
            bool configureAwait = true,
            CancellationToken cancellationToken = default)
        {
            var saveDataRaw = await FromFileAsync(
                filePath,
                configureAwait,
                cancellationToken
                );

            var saveData = JsonConvert.DeserializeObject<SaveData>(saveDataRaw, SaveSystem.SerializerSettings);

            return saveData;
        }

        private static bool IsFileCompressed(FileStream fileStream)
        {
            if (fileStream.Length < 2)
                return false;

            Span<byte> buffer = stackalloc byte[2];

            fileStream.Position = 0;

            fileStream.Read(buffer);

            fileStream.Position = 0;

            return buffer[0] == 0x1F && buffer[1] == 0x8B; //Magic bytes from GZip
        }

        private static async ValueTask<string> TryDecompressAsync(
            FileStream tempFileStream
            )
        {
            StreamReader? streamReader = null!;

            try
            {
                if (GZipHelper.IsCompressed(tempFileStream))
                {
                    using var gZipStream = new GZipStream(tempFileStream, CompressionMode.Decompress);

                    streamReader = new StreamReader(gZipStream);
                }
                else
                    streamReader = new StreamReader(tempFileStream);

                var content = await streamReader.ReadToEndAsync();

                return content;
            }
            catch (Exception ex)
            {
                typeof(SaveLoad).PrintException(ex);

                return string.Empty;
            }
            finally
            {
                streamReader?.Dispose();
            }
        }
    }
}
