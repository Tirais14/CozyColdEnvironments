using System;
using UnityEngine;

#nullable enable
namespace CozyColdEnvironments
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
