using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

#nullable enable
namespace UTIRLib.FileSystem
{
    public class PathNotValidException : TirLibException
    {
        public PathNotValidException()
        {
        }

        public PathNotValidException(string path, string? message = null)
            : base($"Path: \"{path}\"." + (message.IsNotNullOrEmpty() ? $" {message}" : string.Empty))
        {
        }
    }
}
