using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public class ContextedFieldInfo : ContextedMemberInfo
    {
        public readonly FieldInfo value;

        public ContextedFieldInfo(object? context, FieldInfo field) : base(context)
        {
            value = field;
        }

        public void SetValue(object newValue) => value.SetValue(context, newValue);

        public object GetValue() => value.GetValue(context);
    }
}
