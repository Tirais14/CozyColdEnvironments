using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace UTIRLib.Collections
{
    public static class TEnumerable<T>
    {
        public static IEnumerable<T> Empty { get; } = new List<T>();
    }
}
