using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using ICSharpCode;
using CCEnvs.Files;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public static class SaveLoad
    {
        //public static async UniTask<string> ReadFromFile(
        //    string filePath,
        //    bool compressed = true,
        //    CancellationToken cancellationToken = default
        //    )
        //{
        //    Guard.IsNotNull(filePath, nameof(filePath));

        //    var fileInfo = new FileInfo(filePath);  

        //    if (!fileInfo.Exists)
        //    {
        //        if (CCDebug.Instance.IsEnabled)
        //            typeof(SaveLoad).PrintLog($"File: {filePath} not found");

        //        return string.Empty;
        //    }

        //    FileInfo tempFileInfo = null!;

        //    try
        //    {
        //        tempFileInfo = createTempFile(fileInfo);

        //        using var tempFileStream = tempFileInfo.Open(FileMode.Open);

        //        if (tempFileStream.Length == 0)
        //            return string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        typeof(SaveLoad).PrintException(ex);



        //        return string.Empty;
        //    }

        //    static FileInfo createTempFile(FileInfo fileInfo)
        //    {
        //        var path = Path.Combine(fileInfo.DirectoryName, fileInfo.Name, ".temp");

        //        return fileInfo.CopyTo(path);
        //    }
        //}

        //private static void TryDeleteFile(FileInfo? fileInfo)
        //{
        //    if (fileInfo is null)
        //        return;

        //    try
        //    {
        //        fileInfo.Delete();
        //    }
        //    catch (Exception ex)
        //    {
        //        typeof(SaveLoad).PrintException(ex);
        //    }
        //}

        //private static string Decompress(FileStream tempFileStream)
        //{
        //    using var gZipStream = new GZipStream(tempFileStream, CompressionMode.Decompress);

        //    gZipStream.Position = 0;

        //    using var streamReader = new StreamReader(gZipStream);

        //    var content = streamReader.ReadToEnd();

        //    tempFileStream.Position = 0;

        //    return content;
        //}
    }
}
