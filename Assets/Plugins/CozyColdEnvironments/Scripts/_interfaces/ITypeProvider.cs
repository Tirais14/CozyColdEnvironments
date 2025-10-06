#nullable enable
using System;

namespace CCEnvs
{
    public interface ITypeProvider
    {
        Type ObjectType { get; }
    }
}
