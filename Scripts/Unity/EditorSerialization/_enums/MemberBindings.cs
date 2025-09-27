using CCEnvs.Common;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Flags]
    public enum MemberBindings
    {
        Default = NonPublic | Static,
        NonPublic = 2,
        Static = 1
    }

    public static class MemberBindingsExtensions
    {
        public static BindingFlags Unfold(this MemberBindings source)
        {
            if (source.IsFlagsSetted(MemberBindings.NonPublic, MemberBindings.Static))
                return BindingFlagsDefault.All;
            else if (source.IsFlagSetted(MemberBindings.Static))
                return BindingFlagsDefault.StaticPublic;
            else if (source.IsFlagSetted(MemberBindings.NonPublic))
                return BindingFlagsDefault.InstanceAll;

            return BindingFlags.Default;
        }
    }
}
