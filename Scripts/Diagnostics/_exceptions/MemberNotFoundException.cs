using CCEnvs.Reflection;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Diagnostics
{
    public class MemberNotFoundException : CCException
    {
        public MemberNotFoundException()
        {
        }

        public MemberNotFoundException(Type reflectedType,
                                       string? name = null,
                                       BindingFlags? bindingFlags = null)
            :
            base(ReflectedType(reflectedType)
                 +
                 (name is not null ? $" Name = {name}. " : string.Empty) 
                 + 
                 (bindingFlags.HasValue ? $" Binding flags = {bindingFlags.Value}. " : string.Empty))
        {

        }

        public MemberNotFoundException(string message, Exception? innerException = null) 
            :
            base(message, innerException)
        {
        }

        protected static string ReflectedType(Type type)
        {
            return $"Reflected type = {type.GetName()}.";
        }
    }
}
