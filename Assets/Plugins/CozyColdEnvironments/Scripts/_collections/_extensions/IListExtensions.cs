using CCEnvs.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

#nullable enable

namespace CCEnvs.Collections
{
    public static class IListExtensions
    {
        public static T[] ToArrayOrEmpty<T>(this IList<T>? list)
        {
            return list?.ToArray() ?? Array.Empty<T>();
        }
    }
}