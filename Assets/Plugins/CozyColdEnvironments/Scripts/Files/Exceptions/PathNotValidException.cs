using CCEnvs.Diagnostics;
using CCEnvs.Extensions;

#nullable enable
namespace CCEnvs.Files
{
    public class PathNotValidException : CCException
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
