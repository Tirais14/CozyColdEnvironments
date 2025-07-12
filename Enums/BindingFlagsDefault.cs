using System;
using System.Reflection;

namespace UTIRLib
{
    public enum BindingFlagsDefault
    {
        InstancePublic = BindingFlags.Instance | BindingFlags.Public,
        InstanceNonPublic = BindingFlags.Instance | BindingFlags.NonPublic,
        InstanceAll = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
        StaticPublic = BindingFlags.Static | BindingFlags.Public,
        StaticNonPublic = BindingFlags.Static | BindingFlags.NonPublic,
        StaticAll = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
        AllPublic = InstancePublic | StaticPublic,
        AllNonPublic = InstanceNonPublic | StaticNonPublic,
        All = AllPublic | AllNonPublic
    }

    public static class BindingFlagsDefaultExtensions
    {
        public static BindingFlags ToBindingFlags(this BindingFlagsDefault bindingFlagsDefault) =>
            (BindingFlags)Enum.ToObject(typeof(BindingFlags), (int)bindingFlagsDefault);
    }
}