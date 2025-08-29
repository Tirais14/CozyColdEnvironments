using System;

#nullable enable
namespace CozyColdEnvironments.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredPropertyAttribute : RequiredMemberAttribute
    {
    
    }
}
