using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public class ContextedFieldInfo<T> : ContextedMemberInfo
    {
        public readonly FieldInfo value;

        public ContextedFieldInfo(object? context, FieldInfo field) : base(context)
        {
            if (field.FieldType.IsNotType(typeof(T)))
                throw new CCException($"Invalid field type: {typeof(T).GetFullName()}. Expected (or derived from): {field.FieldType.GetFullName()}");

            value = field;
        }

        public static implicit operator ContextedFieldInfo(ContextedFieldInfo<T> source)
        {
            return new ContextedFieldInfo(source.context, source.value);
        }

        public void SetValue(T newValue) => value.SetValue(context, newValue);

        public T GetValue() => (T)value.GetValue(context);

        public TValue GetValueT<TValue>() => value.GetValue(context).As<TValue>();
    }
    public class ContextedFieldInfo : ContextedFieldInfo<object>
    {
        public ContextedFieldInfo(object? context, FieldInfo field)
            : 
            base(context, field)
        {
        }

        [Converter]
        public ContextedFieldInfo<T> Convert<T>()
        {
            return new ContextedFieldInfo<T>(context, value);
        }
    }
}
