#nullable enable
using System;

namespace CCEnvs.FileSystem.ScriptUtils
{
    public interface ITypeProvider
    {
        Type? TypeValue { get; set; }
        bool HasTypeValue { get; }
    }
}
