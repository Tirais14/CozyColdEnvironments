#nullable enable
using CCEnvs.Diagnostics;

namespace CCEnvs.FileSystem
{
    public class FileNameException : TirLibException
    {
        public FileNameException()
        {
        }

        public FileNameException(string? filename) : base($"Filename: {filename ?? "null"}.")
        {
        }

        public FileNameException(string? filename, string message)
            : base($"Filename: {filename ?? "null"}. {message}")
        {
        }
    }
}
