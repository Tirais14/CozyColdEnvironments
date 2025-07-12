using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.FileSystem
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
