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

        /// <summary>Checks for unity or system <see langword="null"/></summary>
        public static bool IsNull<T>([NotNullWhen(false)] this T? obj)
        {
            return new NullValidator<T>(obj).IsNull;
        }

        /// <summary>Checks for unity or system <see langword="null"/></summary>
        public static bool IsNull<T>([NotNullWhen(false)] this T? obj, out NullValidator<T> validationResult)
        {
            validationResult = new NullValidator<T>(obj);

            return validationResult.IsNull;
        }

        /// <summary>Inverted</summary>
        public static bool IsNotNull<T>([NotNullWhen(true)] this T? obj)
        {
            return !new NullValidator<T>(obj).IsNull;
        }
    }
}
