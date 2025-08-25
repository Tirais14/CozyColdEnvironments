using System;
using UnityEngine;

#nullable enable
namespace UTIRLib
{
    [Flags]
    public enum InjectionOptions
    {
        None,
        ThrowIfFailed,
        CacheMember = 2,
        Default = ThrowIfFailed
    }
}
