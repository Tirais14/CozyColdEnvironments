using CCEnvs.Diagnostics;
using CCEnvs.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

#nullable enable
namespace CCEnvs.Saves
{
    public static class SaveLoad
    {
        public static async UniTask<string> FromFileAsync(
            string filePath,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            Guard.IsNotNullOrWhiteSpace(filePath, nameof(filePath));

            await UniTaskHelper.TrySwitchToThreadPool();

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

                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
            }
        }

        public static async UniTask<SaveData?> SaveDataFromFileAsync(
            string filePath,
            bool configureAwait = true,
            CancellationToken cancellationToken = default)
        {
            var saveDataRaw = await FromFileAsync(
                filePath,
                configureAwait,
                cancellationToken
                );

            var saveData = JsonConvert.DeserializeObject<SaveData>(saveDataRaw);

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

        private static async UniTask<string> TryDecompressAsync(
            FileStream tempFileStream
            )
        {
            StreamReader? streamReader = null!;

            try
            {
                if (IsFileCompressed(tempFileStream))
                {
                    using var gZipStream = new GZipStream(tempFileStream, CompressionMode.Decompress);

                    streamReader = new StreamReader(gZipStream);
                }
                else
                    streamReader = new StreamReader(tempFileStream);

                var content = await streamReader.ReadToEndAsync();

                tempFileStream.Position = 0;

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
