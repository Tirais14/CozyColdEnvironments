using System;

#nullable enable
namespace UTIRLib.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredPropertyAttribute : RequiredMemberAttribute
    {
    
    }
}
