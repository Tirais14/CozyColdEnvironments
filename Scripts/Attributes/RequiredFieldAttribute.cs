using System;

#nullable enable
namespace UTIRLib.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RequiredFieldAttribute : RequiredMemberAttribute
    {
    
    }
}
