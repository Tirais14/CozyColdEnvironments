using UnityEngine;

#nullable enable
namespace UTIRLib
{
    /// <summary>
    /// Shortcut to construct objects
    /// </summary>
    public static class C
    {
        public static T[] Array<T>(params T[] values) => values;
    }
}
