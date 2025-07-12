#nullable enable
using UTIRLib.Diagnostics;

namespace UTIRLib.FileSystem
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
