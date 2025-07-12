using System;
using System.Reflection;

#nullable enable
namespace UTIRLib.Diagnostics
{
    public class MemberNotFoundException : TirLibException
    {
        public MemberNotFoundException()
        {
        }

        public MemberNotFoundException(Type targetType) : base($"Target Type: {targetType.Name}.")
        {
        }

        public MemberNotFoundException(Type targetType, MemberType memberType)
            : base($"Target Type: {targetType.Name}, {memberType}.")
        {
        }

        public MemberNotFoundException(Type targetType,
                                       MemberType memberType,
                                       string memberName)
            : base($"Target Type: {targetType.Name}, {memberType}: {memberName}.")
        {
        }

        public MemberNotFoundException(Type targetType,
                                       MemberType memberType,
                                       string memberName,
                                       BindingFlags bindingFlags)
            : base($"Target Type: {targetType.Name}, {memberType}: {memberName}, bindingFlags: {bindingFlags}.")
        {
        }
    }
}
