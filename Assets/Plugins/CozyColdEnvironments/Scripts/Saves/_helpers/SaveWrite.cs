using CCEnvs.Diagnostics;
using CCEnvs.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

#nullable enable
namespace CCEnvs.Saves
{
    public static class SaveWrite
    {
        public static async UniTask ToFileAsync(
            string filePath,
            string fileContent,
            bool backupEnabled = true,
            bool compressionEnabled = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            Guard.IsNotNullOrWhiteSpace(filePath, nameof(filePath));

            await UniTaskHelper.TrySwitchToThreadPool();

            var file = new FileInfo(filePath);

            try
            {
                if (CCDebug.Instance.IsEnabled)
                    typeof(SaveWrite).PrintLog("Writing started");

                if (!file.Directory.Exists)
                {
                    if (CCDebug.Instance.IsEnabled)
                        typeof(SaveWrite).PrintLog($"Directory created: {file.Directory.FullName}");

                    file.Directory.Create();
                }

                await SaveSystem.IOSemaphore.WaitAsync(cancellationToken);

                if (backupEnabled)
                    TryBackupFile(file);

                var tempFile = CreateTempFile(file);

                using var tempFileStream = tempFile.Open(FileMode.OpenOrCreate);

                WriteToFile(tempFileStream, fileContent);

                if (compressionEnabled)
                    TryCompressFile(tempFileStream);

                await tempFileStream.DisposeAsync();

                ReplaceTempFileToFile(tempFile, file);
            }
            finally
            {
                SaveSystem.IOSemaphore.Release();

                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
            }
        }

        private static void WriteToFile(FileStream fileStream, string fileContent)
        {
            var fileWriter = new StreamWriter(fileStream);

            fileWriter.WriteAsync(fileContent);
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

        private static void TryCompressFile(FileStream fileStream)
        {
            if (CCDebug.Instance.IsEnabled)
                typeof(SaveWrite).PrintLog($"Compression started");

            try
            {
                using var gZipStream = new GZipStream(fileStream, CompressionLevel.Optimal);
            }
            catch (Exception ex)
            {
                typeof(SaveWrite).PrintException(ex);
            }
            finally
            {
                fileStream.Position = 0;
            }
        }

        private static FileInfo CreateTempFile(FileInfo fileInfo)
        {
            var path = Path.Combine(fileInfo.DirectoryName, fileInfo.Name + ".temp");

            if (File.Exists(path))
            {
                if (CCDebug.Instance.IsEnabled)
                    typeof(SaveWrite).PrintLog($"Previous temp file deleted: {path}");

                File.Delete(path);
            }

            if (CCDebug.Instance.IsEnabled)
                typeof(SaveWrite).PrintLog($"Temp file created: {fileInfo.FullName}");

            return fileInfo.CopyTo(path);
        }

        private static void TryDeleteFile(FileInfo? file)
        {
            if (file is null
                ||
                !file.Exists)
            {
                return;
            }

            if (CCDebug.Instance.IsEnabled)
                typeof(SaveWrite).PrintLog($"Deleting file: {file.FullName}");

            try
            {
                file.Delete();
            }
            catch (Exception ex)
            {
                typeof(SaveLoad).PrintException(ex);
            }
        }

        private static void TryBackupFile(FileInfo file)
        {
            try
            {
                var backupFile = new FileInfo(Path.ChangeExtension(file.FullName, "bak"));

                TryDeleteFile(backupFile);

                if (CCDebug.Instance.IsEnabled)
                    typeof(SaveWrite).PrintLog($"Backup created: {backupFile.FullName}");

                file.CopyTo(backupFile.FullName);
            }
            catch (Exception ex)
            {
                typeof(SaveWrite).PrintException(ex);
            }
        }
    }
}
