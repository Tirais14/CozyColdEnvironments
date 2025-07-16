using System;
using System.Collections.Generic;

#nullable enable
namespace UTIRLib
{
    public static class ListExtensions
    {
        public static T[] ToArrayOrEmpty<T>(this List<T>? list)
        {
            return list?.ToArray() ?? Array.Empty<T>();
        }
    }
}
