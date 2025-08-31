using CCEnvs.Reflection;
using System;

#nullable enable
namespace CCEnvs.Diagnostics
{
    public class MemberNotFoundException : CCException
    {
        public MemberNotFoundException()
        {
        }

        public MemberNotFoundException(string message, Exception? innerException = null) 
            :
            base(message, innerException)
        {
        }

        protected static string ReflectedType(Type type)
        {
            return $"Reflected type = {type.GetName()}. ";
        }
    }
}
