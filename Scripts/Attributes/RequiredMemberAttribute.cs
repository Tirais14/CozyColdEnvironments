using System;

#nullable enable

namespace UTIRLib.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RequiredMemberAttribute : Attribute
    {
    }
}