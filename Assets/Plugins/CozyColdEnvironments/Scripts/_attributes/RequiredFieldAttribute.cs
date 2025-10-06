using System;

#nullable enable

namespace CCEnvs.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RequiredFieldAttribute : RequiredMemberAttribute
    {
    }
}