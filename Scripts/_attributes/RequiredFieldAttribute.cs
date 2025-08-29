using System;

#nullable enable
namespace CozyColdEnvironments.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RequiredFieldAttribute : RequiredMemberAttribute
    {
    
    }
}
