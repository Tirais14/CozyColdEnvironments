using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

#nullable enable
namespace CCEnvs.Unity.Saves
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

            await UniTask.SwitchToThreadPool();

            var file = new FileInfo(filePath);

            try
            {
                await SaveSystem.readWriteSemaphore.WaitAsync(cancellationToken);

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
                SaveSystem.readWriteSemaphore.Release();

                if (configureAwait)
                    await UniTask.SwitchToMainThread();
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
        }

        private static void TryCompressFile(FileStream fileStream)
        {
            using var gZipStream = new GZipStream(fileStream, CompressionLevel.Optimal);

            fileStream.Position = 0;
        }

        private static FileInfo CreateTempFile(FileInfo fileInfo)
        {
            var path = Path.Combine(fileInfo.DirectoryName, fileInfo.Name + ".temp");

            if (File.Exists(path))
                File.Delete(path);

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

                file.CopyTo(backupFile.FullName);
            }
            catch (Exception ex)
            {
                typeof(SaveWrite).PrintException(ex);
            }
        }
    }
}
