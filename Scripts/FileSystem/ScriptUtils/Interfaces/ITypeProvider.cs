#nullable enable
using System;

namespace CozyColdEnvironments.FileSystem.ScriptUtils
{
    public interface ITypeProvider
    {
        Type? TypeValue { get; set; }
        bool HasTypeValue { get; }
    }
}
