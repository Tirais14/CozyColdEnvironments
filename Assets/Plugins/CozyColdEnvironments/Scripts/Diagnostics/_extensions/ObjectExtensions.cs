#nullable enable
using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#pragma warning disable S3236
namespace CCEnvs.Diagnostics
{
    public static class ObjectExtensions
    {
        public static DebugContext AsDebugContext(this object? value,
                                  DebugArguments arguments = default)
        {
            return new DebugContext(value, arguments);
        }
    }
}
