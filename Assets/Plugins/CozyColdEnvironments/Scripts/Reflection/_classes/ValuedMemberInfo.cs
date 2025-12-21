using CCEnvs.FuncLanguage;
using System;
using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public sealed class ValuedMemberInfo : MemberInfo
    {
        private readonly MemberInfo memberInternal;
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
        public bool ValueGetterIsPublic { get; }
        public bool ValueSetterIsPublic { get; }
        public bool CanRead { get; }
        public bool CanWrite { get; }

        public ValuedMemberInfo(FieldInfo field)
            :
            this((MemberInfo)field)
        {
            UnderlyingType = field.FieldType;
            ValueGetter = (inst) => field.GetValue(inst);
            ValueSetter = (inst, value) => field.SetValue(inst, value);
            ValueGetterIsPublic = field.IsPublic;
            ValueSetterIsPublic = ValueGetterIsPublic;
            CanRead = true;
            CanWrite = true;
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
            ValueGetterIsPublic = prop.GetMethod?.IsPublic ?? false;
            ValueSetterIsPublic = prop.SetMethod?.IsPublic ?? false;
            CanRead = prop.GetMethod is not null;
            CanWrite = prop.SetMethod is not null;
        }

        private ValuedMemberInfo(MemberInfo member)
        {
            declaringType = member.DeclaringType;
            memberType = member.MemberType;
            name = member.Name;
            reflectedType = member.ReflectedType;
        }

        public static implicit operator ValuedMemberInfo(FieldInfo field)
        {
            return new ValuedMemberInfo(field);
        }

        public static implicit operator ValuedMemberInfo(PropertyInfo prop)
        {
            return new ValuedMemberInfo(prop); 
        }

        public static explicit operator FieldInfo(ValuedMemberInfo member)
        {
            return member.ToField().Strict();
        }

        public static explicit operator PropertyInfo(ValuedMemberInfo member)
        {
            return member.ToProperty().Strict();
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

        public Result<FieldInfo> ToField()
        {
            return (memberInternal as FieldInfo, CC.ThrowHelper.InvalidCastException(memberInternal.GetType(), typeof(FieldInfo)));
        }
        public Result<PropertyInfo> ToProperty()
        {
            return (memberInternal as PropertyInfo, CC.ThrowHelper.InvalidCastException(memberInternal.GetType(), typeof(PropertyInfo)));
        }
    }

    public static class ValuedMemberInfoExtensions 
    {
        public static ValuedMemberInfo? GetValuedMember(this Type source, string name)
        {
            CC.Guard.IsNotNullSource(source);

            if (source.GetField(name).Let(out FieldInfo? field))
                return field;
            else if (source.GetProperty(name).Let(out PropertyInfo? prop))
                return prop;

            return null;
        }

        public static ValuedMemberInfo? GetValuedMember(
            this Type source,
            string name,
            BindingFlags bindingFlags)
        {
            CC.Guard.IsNotNullSource(source);

            if (source.GetField(name, bindingFlags).Let(out FieldInfo? field))
                return field;
            else if (source.GetProperty(name, bindingFlags).Let(out PropertyInfo? prop))
                return prop;

            return null;
        }

        public static ValuedMemberInfo[] GetValuedMembers(this Type source)
        {
            CC.Guard.IsNotNullSource(source);

            return source.GetFields()
                .Select(field => new ValuedMemberInfo(field))
                .Concat(source.GetProperties().Select(prop => new ValuedMemberInfo(prop)))
                .ToArray();
        }

        public static ValuedMemberInfo[] GetValuedMembers(this Type source, BindingFlags bindingFlags)
        {
            CC.Guard.IsNotNullSource(source);

            return source.GetFields(bindingFlags)
                .Select(field => new ValuedMemberInfo(field))
                .Concat(source.GetProperties(bindingFlags).Select(prop => new ValuedMemberInfo(prop)))
                .ToArray();
        }
    }
}
