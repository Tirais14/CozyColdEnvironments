#nullable enable
using System;

namespace UTIRLib.FileSystem.ScriptUtils
{
    public interface ITypeProvider
    {
        Type? TypeValue { get; set; }
        bool HasTypeValue { get; }
    }
}
