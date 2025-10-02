#nullable enable
using CCEnvs.Diagnostics;
using System;

namespace CCEnvs.Json
{
    public static class JsonHelper
    {
        /// <returns>true if converter defined in <see cref="Newtonsoft.Json"/> namespace</returns>
        public static bool IsDefaultJsonType(Type type)
        {
            CC.Guard.NullArgument(type, nameof(type));

            return type.Namespace.IsNotNullOrEmpty()
                   &&
                   type.Namespace.ContainsOrdinal(GJson.Namespace);
        }
    }
}
