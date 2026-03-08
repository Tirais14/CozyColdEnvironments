using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCEnvs.Diagnostics;
using CCEnvs.Threading.Tasks;
using Cysharp.Threading.Tasks;

#nullable enable
namespace CCEnvs.Saves
{
    public static class SaveWrite
    {
        public const string DEFAULT_SAVE_EXTENSION = "jgz";

        public static async UniTask ToFileAsync(
            WriteSaveDataToFileParameters parameters,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

#if !PLATFORM_WEBGL
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            var file = new FileInfo(parameters.FilePath);

            if (CCDebug.Instance.IsEnabled)
            {
                typeof(SaveWrite).PrintLog($"Writing started:" +
                    $"{Environment.NewLine}{nameof(parameters.FilePath)}: {parameters.FilePath}" +
                    $"{Environment.NewLine}{nameof(parameters.Backuped)}: {parameters.Backuped}" +
                    $"{Environment.NewLine}{nameof(parameters.Compressed)}: {parameters.Compressed}"
                    );
            }

            try
            {
                await SaveSystem.IOSemaphore.WaitAsync(cancellationToken);

                if (!file.Directory.Exists)
                {
                    if (CCDebug.Instance.IsEnabled)
                        typeof(SaveWrite).PrintLog($"Directory created: {file.Directory.FullName}");

                    file.Directory.Create();
                }

                if (parameters.Backuped)
                    TryBackupFile(file);

                using var tempFileStream = CreateTempFile(file, out var tempFile);

                if (parameters.Compressed)
                {
                    using var gZipStream = new GZipStream(tempFileStream, CompressionLevel.Optimal, leaveOpen: true);

                    await WriteToStreamAsync(gZipStream, parameters.FileContent, cancellationToken);

                    await gZipStream.DisposeAsync();
                }
                else
                    await WriteToStreamAsync(tempFileStream, parameters.FileContent, cancellationToken);

                await tempFileStream.FlushAsync();
                await tempFileStream.DisposeAsync();

                tempFile.Refresh();

                ReplaceTempFileToFile(tempFile, file);
            }
            finally
            {
                SaveSystem.IOSemaphore.Release();

                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
            }
        }

        private static async Task WriteToStreamAsync(
            Stream fileStream,
            string fileContent,
            CancellationToken cancellationToken
            )
        {
            var bytes = Encoding.UTF8.GetBytes(fileContent);

            await fileStream.WriteAsync(bytes, cancellationToken);
        }

        private static void ReplaceTempFileToFile(
            FileInfo tempFile,
            FileInfo file
            )
        {
            TryDeleteFile(file);

            tempFile.MoveTo(file.FullName);

            if (CCDebug.Instance.IsEnabled)
                typeof(SaveWrite).PrintLog($"Writed to file: {file.FullName}");
        }

        //private static GZipStream GetGZipStream(Stream fileStream)
        //{
        //    GZipStream? gZipStream = null;

        //    try
        //    {
        //        gZipStream = new GZipStream(fileStream, CompressionLevel.Optimal, leaveOpen: true);

        //        if (CCDebug.Instance.IsEnabled)
        //            typeof(SaveWrite).PrintLog($"Compressed");

        //        return gZipStream;
        //    }
        //    catch (Exception ex)
        //    {
        //        typeof(SaveWrite).PrintException(ex);

        //        gZipStream?.Dispose();

        //        throw;
        //    }
        //}

        private static FileStream CreateTempFile(FileInfo file, out FileInfo tempFile)
        {
            var path = Path.Combine(file.DirectoryName, file.Name + ".temp");

            tempFile = new FileInfo(path);

            TryDeleteFile(tempFile);

            FileStream tempFileStream = tempFile.Create();

            if (CCDebug.Instance.IsEnabled)
                typeof(SaveWrite).PrintLog($"Temp file created: {file.FullName}");

            return tempFileStream;
        }

        private static void TryDeleteFile(FileInfo? file)
        {
            if (file is null
                ||
                !file.Exists)
            {
                return;
            }

            try
            {
                bool fileExists = file.Exists;

                file.Delete();

                if (fileExists && CCDebug.Instance.IsEnabled)
                    typeof(SaveWrite).PrintLog($"Deleted file: {file.FullName}");
            }
            catch (Exception ex)
            {
                typeof(SaveLoad).PrintException(ex);
            }
        }

        private static void TryBackupFile(FileInfo file)
        {
            if (!file.Exists)
                return;

            try
            {
                var backupFile = new FileInfo(Path.ChangeExtension(file.FullName, "bak"));

                TryDeleteFile(backupFile);

                file.CopyTo(backupFile.FullName);

                if (CCDebug.Instance.IsEnabled)
                    typeof(SaveWrite).PrintLog($"Backup created: {backupFile.FullName}");
            }
            catch (Exception ex)
            {
                typeof(SaveWrite).PrintException(ex);
            }
        }
    }
}
