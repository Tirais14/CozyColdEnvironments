#nullable enable
using System;

namespace CCEnvs.Files.ScriptUtils
{
    public interface ITypeProvider
    {
        Type? TypeValue { get; set; }
        bool HasTypeValue { get; }
    }
}
