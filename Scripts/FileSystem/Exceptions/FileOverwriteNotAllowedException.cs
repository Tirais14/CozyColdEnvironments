using CozyColdEnvironments.Diagnostics;

#nullable enable
namespace CozyColdEnvironments.FileSystem
{
    public class FileOverwriteNotAllowedException : TirLibException
    {
        public FileOverwriteNotAllowedException()
        {
        }

        public FileOverwriteNotAllowedException(string path) : base($"Path: \"{path}\".")
        {
        }
    }
}
