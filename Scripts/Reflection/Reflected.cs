#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Reflection.Data;
using System;
using System.Reflection;
using static CCEnvs.BindingFlagsDefault;

namespace CCEnvs.Reflection
{
    /// <summary>
    /// Used for convenient type handling using reflection
    /// </summary>
    public sealed class Reflected : IEquatable<Reflected>
    {
        public object? Target { get; }
        public Type TargetType { get; }

        public Reflected(Type targetType)
        {
            Target = null;
            TargetType = targetType;
        }

        public Reflected(object target)
        {
            Target = target;
            TargetType = target.GetType();
        }

        public static Reflected T<T>()
        {
            return new Reflected(typeof(T));
        }

        public static bool operator ==(Reflected left, Reflected right)
        {
            return ReferenceEquals(left, right) || left.Equals(right);
        }

        public static bool operator !=(Reflected left, Reflected right)
        {
            return !(left == right);
        }

        /// <exception cref="FieldNotFoundException"></exception>
        public FieldInfo Field(string name, bool nonPublic = false)
        {
            Validate.StringArgument(name, nameof(name));

            BindingFlags bindings = ResolveBindingFlags(nonPublic);
            return TargetType.ForceGetField(name, bindings) 
                ??
                throw new FieldNotFoundException(TargetType, name, bindings);
        }

        public object FieldGet(string name, bool nonPublic = false)
        {
            return Field(name, nonPublic).GetValue(Target);
        }

        public void FieldSet(string name, object value, bool nonPublic = false)
        {
            Field(name, nonPublic).SetValue(Target, value);
        }

        /// <exception cref="PropertyNotFoundException"></exception>
        public PropertyInfo Property(string name, bool nonPublic = false)
        {
            Validate.StringArgument(name, nameof(name));

            BindingFlags bindings = ResolveBindingFlags(nonPublic);
            return TargetType.ForceGetProperty(name, bindings)
                   ??
                   throw new PropertyNotFoundException(TargetType, name, bindings);
        }

        public object PropertyGet(string name, bool nonPublic = false)
        {
            return Property(name, nonPublic).GetValue(Target);
        }

        public void PropertySet(string name, object value, bool nonPublic = false)
        {
            Property(name, nonPublic).SetValue(Target, value);
        }

        public EventInfo Event(string name, bool nonPublic = false)
        {
            Validate.StringArgument(name, nameof(name));

            BindingFlags bindings = ResolveBindingFlags(nonPublic);

            return TargetType.GetEvent(name, bindings)
                   ?? 
                   throw new EventNotFoundException(TargetType, name, bindings);
        }

        public void EventAdd(string name, Delegate handler, bool nonPublic = false)
        {
            Event(name, nonPublic).AddEventHandler(Target, handler);
        }

        public void EventRemove(string name, Delegate handler, bool nonPublic = false)
        {
            Event(name, nonPublic).RemoveEventHandler(Target, handler);
        }

        /// <exception cref="MethodNotFoundException"></exception>
        public MethodInfo Method(string name,
                                 ExplicitArguments args = default,
                                 bool nonPublic = false)
        {
            Validate.StringArgument(nameof(name), name);

            if (args.IsDefault())
                args = ExplicitArguments.Empty;

            BindingFlags bindings = ResolveBindingFlags(nonPublic);
            return TargetType.ForceGetMethod(
                name,
                args.GetTypes(),
                bindings)
                ??
                throw new MethodNotFoundException(
                    TargetType,
                    name,
                    bindings,
                    new CCParameters(args.GetTypes()));
        }

        public object MethodInvoke(string name,
                                   ExplicitArguments args = default,
                                   bool nonPublic = false)
        {
            Validate.StringArgument(nameof(name), name);

            if (args.IsDefault())
                args = ExplicitArguments.Empty;

            BindingFlags bindings = ResolveBindingFlags(nonPublic);
            MethodInfo method = TargetType.ForceGetMethod(name,
                                                          args.GetTypes(),
                                                          bindings)
                ??
                throw new MethodNotFoundException(TargetType,
                                                  name,
                                                  bindings,
                                                  new CCParameters(args.GetTypes()));

            return method.Invoke(Target, (object?[])args);
        }

        public object Cast(Type toType)
        {
            Validate.ArgumentNull(toType, nameof(toType));

            if (Target.IsNull())
                throw new LogicException($"Cannot cast static class {TargetType.GetName()}.");

            if (TargetType.IsType(toType))
                return Target;

            MethodInfo? op = TargetType.GetOverloadedCastOperator(toType);

            if (op is not null)
                return op.Invoke(null!, CC.C.Array(Target));

            return Convert.ChangeType(Target, toType);
        }
        public T Cast<T>() => (T)Cast(typeof(T));

        public bool Equals(Reflected? other)
        {
            if (other is null)
                return false;

            return Equals(TargetType, other.TargetType)
                   && 
                   TargetType.Equals(other.TargetType);
        }

        public override bool Equals(object obj)
        {
            return obj is Reflected typed && Equals(typed);
        }

        public override int GetHashCode() => HashCode.Combine(Target, TargetType);

        private BindingFlags ResolveBindingFlags(bool nonPublic)
        {
            if (nonPublic)
            {
                if (Target.IsNull())
                    return StaticAll;
                else
                    return All;
            }

            if (Target.IsNull())
                return StaticPublic;

            return AllPublic;
        }
    }
}
