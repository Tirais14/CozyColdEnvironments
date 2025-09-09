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
            CC.Validate.StringArgument(name, nameof(name));

            BindingFlags bindings = ResolveBindingFlags(nonPublic);
            return TargetType.ForceGetField(name, bindings) 
                ??
                throw new FieldNotFoundException(TargetType, name, bindings);
        }

        public object FieldGet(string name, bool nonPublic = false)
        {
            return Field(name, nonPublic).GetValue(Target);
        }
        public T FieldGet<T>(string name, bool nonPublic = false)
        {
            object untyped = FieldGet(name, nonPublic);

            if (untyped is not T typed)
            {
                CC.Throw.InvalidCast(untyped.GetType(), typeof(T));
                return default;
            }

            return typed;
        }

        public void FieldSet(string name, object value, bool nonPublic = false)
        {
            Field(name, nonPublic).SetValue(Target, value);
        }

        /// <exception cref="PropertyNotFoundException"></exception>
        public PropertyInfo Property(string name, bool nonPublic = false)
        {
            CC.Validate.StringArgument(name, nameof(name));

            BindingFlags bindings = ResolveBindingFlags(nonPublic);
            return TargetType.ForceGetProperty(name, bindings)
                   ??
                   throw new PropertyNotFoundException(TargetType, name, bindings);
        }

        public object PropertyGet(string name, bool nonPublic = false)
        {
            return Property(name, nonPublic).GetValue(Target);
        }

        public T PropertyGet<T>(string name, bool nonPublic = false)
        {
            object untyped = PropertyGet(name, nonPublic);
            if (untyped is not T typed)
            {
                CC.Throw.InvalidCast(untyped.GetType(), typeof(T));
                return default;
            }

            return typed;
        }

        public void PropertySet(string name, object value, bool nonPublic = false)
        {
            Property(name, nonPublic).SetValue(Target, value);
        }

        public EventInfo Event(string name, bool nonPublic = false)
        {
            CC.Validate.StringArgument(name, nameof(name));

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
            CC.Validate.StringArgument(nameof(name), name);

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

        public object? MethodInvoke(string name,
                                    ExplicitArguments args = default,
                                    bool nonPublic = false)
        {
            CC.Validate.StringArgument(nameof(name), name);

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
        public T? MethodInvoke<T>(string name,
                                 ExplicitArguments args = default,
                                 bool nonPublic = false)
        {
            object? untyped = MethodInvoke(name, args, nonPublic);

            if (untyped.IsDefault())
                return default;

            if (untyped is not T typed)
            {
                CC.Throw.InvalidCast(untyped.GetType(), typeof(T));
                return default;
            }

            return typed;
        }

        public object ChangeType(Type toType)
        {
            CC.Validate.ArgumentNull(toType, nameof(toType));

            if (Target is null)
                CC.Throw.InvalidCast(toType, "Cannot not convert null object.");

            return CCConvert.ChangeType(Target, toType);
        }

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
