using CCEnvs.FuncLanguage;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public sealed class ValuedMemberInfo : MemberInfo
    {
        private readonly Type declaringType;
        private readonly MemberTypes memberType;
        private readonly string name;
        private readonly Type reflectedType;

        public override Type DeclaringType => declaringType;
        public override MemberTypes MemberType => memberType;
        public override string Name => name;
        public override Type ReflectedType => reflectedType;
        public Type UnderlyingType { get; }
        public Maybe<MethodInfo> ValueGetMethod { get; }
        public Maybe<MethodInfo> ValueSetMethod { get; }
        public Func<object?, object?> ValueGetter { get; }
        public Action<object?, object?> ValueSetter { get; }

        public ValuedMemberInfo(FieldInfo field)
            :
            this((MemberInfo)field)
        {
            UnderlyingType = field.FieldType;
            ValueGetter = (inst) => field.GetValue(inst);
            ValueSetter = (inst, value) => field.SetValue(inst, value);
        }

        public ValuedMemberInfo(PropertyInfo prop)
            :
            this((MemberInfo)prop)
        {
            UnderlyingType = prop.PropertyType;
            ValueGetMethod = prop.GetMethod;
            ValueSetMethod = prop.SetMethod;
            ValueGetter = (inst) => prop.GetValue(inst);
            ValueSetter = (inst, value) => prop.SetValue(inst, value);
        }

        private ValuedMemberInfo(MemberInfo member)
        {
            declaringType = member.DeclaringType;
            memberType = member.MemberType;
            name = member.Name;
            reflectedType = member.ReflectedType;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
    }
}
