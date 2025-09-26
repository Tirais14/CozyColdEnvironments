#nullable enable
using CCEnvs.Cacheables;
using CCEnvs.Common;
using CCEnvs.Conversations;
using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using CCEnvs.Reflection.Data;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using static CCEnvs.BindingFlagsDefault;

#pragma warning disable IDE1006
#pragma warning disable S4035
namespace CCEnvs.Reflection
{
    /// <summary>
    /// Used for convenient type handling using reflection
    /// </summary>
    public class Reflected : IEquatable<Reflected>
    {
        [Flags]
        public enum Settings 
        {
            Default = IncludeNonPublic | OnlyStaticWhenTargetIsNull,
            None = 0,
            IncludeNonPublic = 1,
            OnlyStaticWhenTargetIsNull = 2,
            DisallowCaching = 4
        }

        private object? target;

        public LazyCC<ImmutableArray<FieldInfo>> AllFields { get; }
        public LazyCC<ImmutableArray<PropertyInfo>> AllProperties { get; }
        public LazyCC<ImmutableArray<MethodInfo>> AllMethods { get; }
        public LazyCC<ImmutableArray<EventInfo>> AllEvents { get; }
        public LazyCC<ImmutableArray<FieldInfo>> PublicFields { get;}
        public LazyCC<ImmutableArray<PropertyInfo>> PublicProperties { get; }
        public LazyCC<ImmutableArray<MethodInfo>> PublicMethods { get; }
        public LazyCC<ImmutableArray<EventInfo>> PublicEvents { get;}

        public object? Target {
            get => target;
            set
            {
                target = value;

                if (value is not null)
                    TargetType = target?.GetType()!;
            }
        }
        public Type TargetType { get; private set; }
        public Settings settings { get; }
        public bool IncludeNonPublic { get;}
        public bool DisallowCaching { get; }

        protected Reflected(Type targetType, object? target, Settings settings = default)
        {
            this.target = target;
            TargetType = targetType;
            this.settings = settings;

            IncludeNonPublic = settings.IsFlagSetted(Settings.IncludeNonPublic);
            DisallowCaching = settings.IsFlagSetted(Settings.DisallowCaching);

            AllFields = new LazyCC<ImmutableArray<FieldInfo>>(() => TargetType.ForceGetFields(GetBindingFlags()).ToImmutableArray());
            AllProperties = new LazyCC<ImmutableArray<PropertyInfo>>(() => TargetType.ForceGetProperties(GetBindingFlags()).ToImmutableArray());
            AllMethods = new LazyCC<ImmutableArray<MethodInfo>>(() => TargetType.ForceGetMethods(GetBindingFlags()).ToImmutableArray());
            AllEvents = new LazyCC<ImmutableArray<EventInfo>>(() => TargetType.ForceGetMembers<EventInfo>(GetBindingFlags()).ToImmutableArray());

            PublicFields = new LazyCC<ImmutableArray<FieldInfo>>(() => AllFields.Value.Where(field => field.IsPublic).ToImmutableArray());
            PublicProperties = new LazyCC<ImmutableArray<PropertyInfo>>(() => AllProperties.Value.Where(prop => prop.GetMethod.IsPublic).ToImmutableArray());
            PublicMethods = new LazyCC<ImmutableArray<MethodInfo>>(() => AllMethods.Value.Where(method => method.IsPublic).ToImmutableArray());
            PublicEvents = new LazyCC<ImmutableArray<EventInfo>>(() => AllEvents.Value.Where(ev => ev.AddMethod.IsPublic || ev.RemoveMethod.IsPublic).ToImmutableArray());
        }

        public Reflected(Type targetType, Settings settings = default)
            :
            this(targetType, target: null, settings)
        {
        }

        public Reflected(object target, Settings settings = default)
            :
            this(target.GetType(), target, settings)
        {
        }

        public static Reflected T<T>(Settings settings = default)
        {
            return new Reflected(typeof(T), settings);
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
        public ContextedFieldInfo Field(string name)
        {
            CC.Validate.StringArgument(name, nameof(name));

            IEnumerable<FieldInfo> fields = IncludeNonPublic ? AllFields.Value : PublicFields.Value;

            FieldInfo found = fields.FirstOrDefault(x => x.Name.EqualsOrdinal(name))
                              ??
                              throw new FieldNotFoundException(TargetType, name, GetBindingFlags());

            return new ContextedFieldInfo(Target, found);
        }
        public ContextedFieldInfo<T> Field<T>(string name)
        {
            return Field(name).Convert<T>();
        }
        public ContextedFieldInfo Field(Type type)
        {
            CC.Validate.ArgumentNull(type, nameof(type));

            if (TypeCache.Fields.TryGetValue(
                new FieldKey(TargetType, type),
                out FieldInfo found)
                )
                return new ContextedFieldInfo(Target, found);

            IEnumerable<FieldInfo> fields = IncludeNonPublic ? AllFields.Value : PublicFields.Value;

            FieldInfo[] filteredFields = (from field in fields
                                          where field.FieldType.IsType(type)
                                          select field).ToArray();

            if (filteredFields.Length > 1
                &&
                fields.FirstOrDefault(x => x.FieldType == type) is FieldInfo preciseMatch
                )
                return new ContextedFieldInfo(Target, preciseMatch);

            found = (fields.FirstOrDefault()
                    ??
                    throw new FieldNotFoundException(TargetType, type, GetBindingFlags()))
                    .TryCacheMember();

            return new ContextedFieldInfo(Target, found);
        }
        public ContextedFieldInfo<T> Field<T>() => Field(typeof(T)).Convert<T>();

        /// <exception cref="PropertyNotFoundException"></exception>
        public ContextedPropertyInfo Property(string name)
        {
            CC.Validate.StringArgument(name, nameof(name));

            if (TypeCache.Properties.TryGetValue(
                new FieldKey(TargetType, name),
                out PropertyInfo found)
                )
                return new ContextedPropertyInfo(Target, found);

            IEnumerable<PropertyInfo> props = IncludeNonPublic
                                              ? 
                                              AllProperties.Value
                                              : 
                                              PublicProperties.Value;

            found = (props.FirstOrDefault(x => x.Name.EqualsOrdinal(name))
                    ??
                    throw new PropertyNotFoundException(TargetType, name, GetBindingFlags()))
                    .TryCacheMember();

            return new ContextedPropertyInfo(Target, found);
        }
        public ContextedPropertyInfo<T> Property<T>(string name)
        {
            return Property(name).Convert<T>();
        }
        public ContextedPropertyInfo Property(Type type)
        {
            CC.Validate.ArgumentNull(type, nameof(type));

            if (TypeCache.Properties.TryGetValue(
                    new FieldKey(TargetType, type),
                    out PropertyInfo found)
                    )
                return new ContextedPropertyInfo(Target, found);

            IEnumerable<PropertyInfo> props = IncludeNonPublic
                                              ?
                                              AllProperties.Value
                                              :
                                              PublicProperties.Value;

            props = (from prop in props
                     where prop.PropertyType.IsType(type)
                     select prop)
                     .ToArray();

            if (props.FirstOrDefault(x => x.PropertyType == type) is PropertyInfo preciseMatch)
                return new ContextedPropertyInfo(Target, preciseMatch);

            found = (props.FirstOrDefault()
                    ??
                    throw new PropertyNotFoundException(TargetType, type, GetBindingFlags()))
                    .TryCacheMember();

            return new ContextedPropertyInfo(Target, found);
        }
        public ContextedPropertyInfo<T> Property<T>()
        {
            return Property(typeof(T)).Convert<T>();
        }

        /// <exception cref="MethodNotFoundException"></exception>
        public ContextedMethodInfo Method(string name,
                                 ExplicitArguments args = default,
                                 bool ignoreOptionalParameters = false)
        {
            CC.Validate.StringArgument(name, nameof(name));

            if (TypeCache.Methods.TryGetValue(
                new MethodKey(TargetType, (CCParameters)args, args.GetParameterModifiers()),
                out MethodInfo found)
                )
                return new ContextedMethodInfo(Target, found);

            IEnumerable<MethodInfo> methods = IncludeNonPublic
                                              ?
                                              AllMethods.Value
                                              :
                                              PublicMethods.Value;

            found = ((from method in methods
                      where method.Name.EqualsOrdinal(name)
                      where method.GetCCParameters(ignoreOptionalParameters) == ((CCParameters)args)
                      select method)
                     .FirstOrDefault()
                     ??
                     throw new MethodNotFoundException(TargetType, name, GetBindingFlags()))
                     .TryCacheMember();

            return new ContextedMethodInfo(Target, found);
        }

        public ContextedEventInfo Event(string name)
        {
            CC.Validate.StringArgument(name, nameof(name));

            IEnumerable<EventInfo> events = IncludeNonPublic
                                            ?
                                            AllEvents.Value
                                            :
                                            PublicEvents.Value;

            return new ContextedEventInfo(Target, 
                   events.FirstOrDefault(x => x.Name.EqualsOrdinal(name))
                   ??
                   throw new EventNotFoundException(TargetType, name, GetBindingFlags()));
        }
        public ContextedEventInfo<T> Event<T>(string name)
            where T : Delegate
        {
            return Event(name).Convert<T>();
        }

        public object TransformType(Type toType)
        {
            CC.Validate.ArgumentNull(toType, nameof(toType));

            if (Target is null)
                CC.Throw.InvalidCast(toType, "Cannot not convert null object.");

            Target = TypeTransformer.DoTransform(Target, toType);

            if (Target is null)
                throw new DataAccessException(nameof(Target));

            return Target;
        }
        [DebuggerStepThrough]
        public T TransformType<T>() => (T)TransformType(typeof(T));

        /// <summary>
        /// Cast target object in specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? As<T>() => (T?)Target;

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

        private BindingFlags GetBindingFlags()
        {
            if (Target is null
                && 
                settings.IsFlagSetted(Settings.OnlyStaticWhenTargetIsNull)
                )
                return IncludeNonPublic ? StaticAll : StaticPublic;

            return IncludeNonPublic ? All : AllPublic;
        }
    }

    public class Reflected<T> : Reflected
    {
        public Reflected(object? target, Settings settings = Settings.None)
            : 
            base(typeof(T), target, settings)
        {
        }
    }
}
