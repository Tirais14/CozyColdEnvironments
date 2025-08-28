using System;
using UTIRLib.Reflection.ObjectModel;

#nullable enable
namespace UTIRLib
{
    /// <summary>
    /// Shortcut to construct objects
    /// </summary>
    public static class TC
    {
        public static T[] Array<T>(params T[] values) => values;

        public static Signature Signature(params Type[] types)
        {
            return new Signature(types);
        }
    }
}
