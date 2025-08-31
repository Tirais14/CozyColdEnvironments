using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.FileSystem
{
    public class FileOverwriteNotAllowedException : CCException
    {
        public FileOverwriteNotAllowedException()
        {
        }

        public FileOverwriteNotAllowedException(string path) : base($"Path: \"{path}\".")
        {
        }
    }
}
