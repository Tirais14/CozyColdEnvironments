using System;

#nullable enable
namespace CCEnvs.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredPropertyAttribute : RequiredMemberAttribute
    {
    
    }
}
