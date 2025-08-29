using CozyColdEnvironments.Diagnostics;
using CozyColdEnvironments.Extensions;

#nullable enable
namespace CozyColdEnvironments.FileSystem
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
