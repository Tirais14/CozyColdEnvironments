using CCEnvs.Diagnostics;
using CCEnvs.Extensions;

#nullable enable
namespace CCEnvs.FileSystem
{
    public class PathNotValidException : CCEException
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
