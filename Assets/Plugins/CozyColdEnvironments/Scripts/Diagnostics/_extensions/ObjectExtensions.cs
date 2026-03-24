using System;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.Diagnostics
{
    public static class ObjectExtensions
    {
        public static T ThrowIfNull<T>(
            this T? source,
            string? paramName = null,
            Exception? ex = null
            )
        {
            if (source.IsNotNull())
                return source;

            if (ex == null)
                throw new ArgumentNullException(paramName);

            throw ex;
        }
    }
}
