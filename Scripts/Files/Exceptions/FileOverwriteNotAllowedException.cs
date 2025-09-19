using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.Files
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
