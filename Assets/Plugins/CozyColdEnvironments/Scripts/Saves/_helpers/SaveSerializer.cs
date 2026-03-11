using CCEnvs.IO;
using CCEnvs.IO.Compression;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Saves
{
    public static class SaveSerializer
    {
        public static async ValueTask<SaveData?> DeserializeAsync(
            string? serialized,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (serialized.IsNullOrWhiteSpace())
                return null;

            await SaveSystem.SerializingSemaphore.WaitAsync();

            try
            {
                if (GZipHelper.IsCompressed(serialized))
                {
                    using var memStream = await StreamFactory.CreateMemoryStreamAsync(
                        serialized.AsMemory(),
                        cancellationToken: cancellationToken
                        );

                    using var gZipStream = new GZipStream(memStream, mode: CompressionMode.Decompress);

                    using var streamReader = new StreamReader(gZipStream);

#if !PLATFORM_WEBGL
                    serialized = await streamReader.ReadToEndAsync();
#else
                    serialized = streamReader.ReadToEnd();
#endif
                }

                return JsonConvert.DeserializeObject<SaveData>(serialized, SaveSystem.SerializerSettings);
            }
            catch (Exception ex)
            {
                typeof(SaveSerializer).PrintException(ex);
                return null;
            }
            finally
            {
                SaveSystem.SerializingSemaphore.Release();
            }
        }

        public static async ValueTask<string> SerializeAsync(
            SaveData? saveData,
            bool compressed = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (saveData is null)
                return string.Empty;

            await SaveSystem.SerializingSemaphore.WaitAsync();

            try
            {
                var serialized = JsonConvert.SerializeObject(
                    saveData,
                    SaveSystem.SerializerSettings
                    )
                    ??
                    string.Empty;

                if (serialized.IsNullOrWhiteSpace())
                    return serialized;

                using var memStream = StreamFactory.CreateMemoryStream(serialized);

                using var gZipStream = new GZipStream(memStream, CompressionLevel.Optimal);

                using var streamReader = new StreamReader(gZipStream);

#if !PLATFORM_WEBGL
                return await streamReader.ReadToEndAsync();
#else
                return streamReader.ReadToEnd();
#endif
            }
            catch (Exception ex)
            {
                typeof(SaveSerializer).PrintException(ex);
                return string.Empty;
            }
            finally
            {
                SaveSystem.SerializingSemaphore.Release();
            }
        }
    }
}
