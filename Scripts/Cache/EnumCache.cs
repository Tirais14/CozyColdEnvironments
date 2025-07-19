using System;
using System.Linq;

#nullable enable
namespace UTIRLib.Utils
{
    public static class EnumCache<T>
        where T : Enum
    {
        private static T[]? values;

        public static T[] Values {
            get {
                values ??= Enum.GetValues(typeof(T)).Cast<T>().ToArray();

                return values;
            }
        }

        public static void ClearValues() => values = null;
    }
}
