#nullable enable
using CCEnvs.Attributes;
using System.Reflection;

namespace CCEnvs.Reflection
{
    public class ContextedPropertyInfo<T> : ContextedMemberInfo
    {
        public readonly PropertyInfo value;

        public ContextedPropertyInfo(object? context, PropertyInfo property) : base(context)
        {
            value = property;
        }

        public static implicit operator ContextedPropertyInfo(ContextedPropertyInfo<T> source)
        {
            return new ContextedPropertyInfo(source.context, source.value);
        }

        public T GetValue() => (T)value.GetValue(context);

        public void SetValue(T newValue) => value.SetValue(context, newValue); 
    }
    public class ContextedPropertyInfo : ContextedPropertyInfo<object>
    {
        public ContextedPropertyInfo(object? context, PropertyInfo property)
            : 
            base(context, property)
        {
        }

        [Converter]
        public ContextedPropertyInfo<T> Convert<T>()
        {
            return new ContextedPropertyInfo<T>(context, value);
        }
    }
}
