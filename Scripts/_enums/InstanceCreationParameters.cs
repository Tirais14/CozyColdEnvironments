using System;
using UnityEngine;

#nullable enable
namespace UTIRLib
{
    [Flags]
    public enum InstanceCreationParameters
    {
        None,
        CacheConstructor,
        ThrowIfNotFound = 2,
        Default = ThrowIfNotFound,
    }
}
