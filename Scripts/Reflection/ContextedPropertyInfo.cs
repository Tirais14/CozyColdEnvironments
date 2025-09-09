#nullable enable
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace CCEnvs.Reflection
{
    public class ContextedPropertyInfo : ContextedMemberInfo
    {
        public readonly PropertyInfo value;

        public ContextedPropertyInfo(object? context, PropertyInfo property) : base(context)
        {
            value = property;
        }

        public object GetValue() => value.GetValue(context);

        public void SetValue(object newValue) => value.SetValue(context, newValue); 
    }
}
