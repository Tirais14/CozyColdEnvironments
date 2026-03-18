using System;

#nullable enable
namespace CCEnvs.Reflection
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PropertyIDAttribute : MemberIDAttribute
    {
        public PropertyIDAttribute(string id)
            :
            base(id)
        {
        }
    }
}
