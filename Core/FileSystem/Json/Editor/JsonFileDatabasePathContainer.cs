using System;

namespace UTIRLib.FileSystem.Json.Editor
{
    [Obsolete]
    public static class JsonFileDatabasePathContainer
    {
        //public static void Create(string targetDirectoryPath, string fileName, bool owerwrite = false)
        //{
        //    CheckTargetDirectory(targetDirectoryPath);

        //    string[] databaseFiles = GetDatabaseFilesExcludeCore();

        //    FileInfoExtended databaseCoreFileInfo =
        //        new(targetDirectoryPath, fileName, ".json");
        //    JsonFile<string[]> databaseCoreJsonFile = new(databaseFiles, databaseCoreFileInfo);
        //    databaseCoreJsonFile.Save(owerwrite);

        //    UnityEditor.AssetDatabase.Refresh();
        //}

        //private static string[] GetDatabaseFilesExcludeCore() =>
        //    GetDatabaseFiles().
        //    Where(SearchDatabaseFilesFilter).
        //    Select(ConvertFileInfoToPath).
        //    ToArray();

        //private static FileInfoExtended[] GetDatabaseFiles() =>
        //    new DirectoryInfoExtended(ResourcesDirectory.AssetDatabases).
        //    GetAllFilesExtended("*" + ".json");

        //private static bool SearchDatabaseFilesFilter(FileInfoExtended file) =>
        //    !file.FileName.Equals(AssetDatabaseNames.CORE, System.StringComparison.CurrentCulture);

        //private static string ConvertFileInfoToPath(FileInfoExtended file) =>
        //    file.
        //    ConvertToRelativePathQuery(AppDirectory.rootDirectoryPath).
        //    ConvertToUniversalPathQuery().FilePath;

        //private static void CheckTargetDirectory(string directoryPath)
        //{
        //    DirectoryInfoExtended directoryInfo = new(directoryPath);
        //    if (!directoryInfo.Base.Exists) {
        //        directoryInfo.Base.Create();
        //    }
        //}
    }
}