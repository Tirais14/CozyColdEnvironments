using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Reflection
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class FieldIDAttribute : MemberIDAttribute
    {
        public FieldIDAttribute(string id)
            :
            base(id)
        {
        }
    }
}
