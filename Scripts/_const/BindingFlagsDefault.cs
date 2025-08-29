using System.Reflection;

namespace CozyColdEnvironments
{
    public static class BindingFlagsDefault
    {
        public const BindingFlags InstancePublic = BindingFlags.Instance | BindingFlags.Public;
        public const BindingFlags InstanceNonPublic = BindingFlags.Instance | BindingFlags.NonPublic;
        public const BindingFlags InstanceAll = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        public const BindingFlags StaticPublic = BindingFlags.Static | BindingFlags.Public;
        public const BindingFlags StaticNonPublic = BindingFlags.Static | BindingFlags.NonPublic;
        public const BindingFlags StaticAll = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        public const BindingFlags AllPublic = InstancePublic | StaticPublic;
        public const BindingFlags AllNonPublic = InstanceNonPublic | StaticNonPublic;
        public const BindingFlags All = AllPublic | AllNonPublic;
    }
}