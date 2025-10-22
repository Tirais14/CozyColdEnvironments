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

        private object? instance;

        public LazyCC<ImmutableArray<FieldInfo>> AllFields { get; }
        public LazyCC<ImmutableArray<PropertyInfo>> AllProperties { get; }
        public LazyCC<ImmutableArray<MethodInfo>> AllMethods { get; }
        public LazyCC<ImmutableArray<EventInfo>> AllEvents { get; }
        public LazyCC<ImmutableArray<FieldInfo>> PublicFields { get;}
        public LazyCC<ImmutableArray<PropertyInfo>> PublicProperties { get; }
        public LazyCC<ImmutableArray<MethodInfo>> PublicMethods { get; }
        public LazyCC<ImmutableArray<EventInfo>> PublicEvents { get;}

        public object? Instance {
            get => instance;
            set
            {
                instance = value;

                if (value is not null)
                    InstanceType = instance?.GetType()!;
            }
        }
        public Type InstanceType { get; private set; } = null!;
        public Settings settings { get; } = Settings.Default;
        public bool IncludeNonPublic => settings.IsFlagSetted(Settings.IncludeNonPublic);
        public bool DisallowCaching => settings.IsFlagSetted(Settings.DisallowCaching);

        protected Reflected()
        {
            AllFields = new LazyCC<ImmutableArray<FieldInfo>>(() => InstanceType.ForceGetFields(GetBindingFlags()).ToImmutableArray());
            AllProperties = new LazyCC<ImmutableArray<PropertyInfo>>(() => InstanceType.ForceGetProperties(GetBindingFlags()).ToImmutableArray());
            AllMethods = new LazyCC<ImmutableArray<MethodInfo>>(() => InstanceType.ForceGetMethods(GetBindingFlags()).ToImmutableArray());
            AllEvents = new LazyCC<ImmutableArray<EventInfo>>(() => InstanceType.ForceGetMembers<EventInfo>(GetBindingFlags()).ToImmutableArray());

            PublicFields = new LazyCC<ImmutableArray<FieldInfo>>(() => AllFields.Value.Where(field => field.IsPublic).ToImmutableArray());
            PublicProperties = new LazyCC<ImmutableArray<PropertyInfo>>(() => AllProperties.Value.Where(prop => prop.GetMethod.IsPublic).ToImmutableArray());
            PublicMethods = new LazyCC<ImmutableArray<MethodInfo>>(() => AllMethods.Value.Where(method => method.IsPublic).ToImmutableArray());
            PublicEvents = new LazyCC<ImmutableArray<EventInfo>>(() => AllEvents.Value.Where(ev => ev.AddMethod.IsPublic || ev.RemoveMethod.IsPublic).ToImmutableArray());
        }

        public Reflected(Type instanceType)
            :
            this()
        {            
            InstanceType = instanceType;
        }

        public Reflected(Type instanceType, object? instance)
            :
            this(instanceType)
        {
            this.instance = instance;
        }

        public Reflected(Type instanceType, Settings settings)
            :
            this(instanceType)
        {
            this.settings = settings;
        }

        public Reflected(Type instanceType, object? instance, Settings settings)
            :
            this(instanceType, settings)
        {
            this.instance = instance;
        }

        public Reflected(TypeSearchArguments typeSearchArguments)
            :
            this()
        {
            InstanceType = TypeSearch.FindTypeInAppDomain(typeSearchArguments, throwOnError: true);
        }

        public Reflected(TypeSearchArguments typeSearchArguments, object? instance)
            :
            this(typeSearchArguments)
        {
            this.instance = instance;
        }

        public Reflected(TypeSearchArguments typeSearchArguments, object? instance, Settings settings)
            :
            this(typeSearchArguments, instance)
        {
            this.settings = settings;
        }

        public Reflected(TypeSearchArguments typeSearchArguments, Settings settings)
            :
            this(typeSearchArguments)
        {
            this.settings = settings;
        }

        public Reflected(object instance)
            :
            this(instance?.GetType() ?? throw new ArgumentNullException(nameof(instance)), instance)
        {
        }

        public Reflected(object instance, Settings settings)
            :
            this(instance?.GetType() ?? throw new ArgumentNullException(nameof(instance)), instance, settings)
        {
        }

        public static bool operator ==(Reflected? left, Reflected? right)
        {
            if (ReferenceEquals(left, right))
                return true;

            return left is not null && left.Equals(right);
        }

        public static bool operator !=(Reflected? left, Reflected? right)
        {
            return !(left == right);
        }

        /// <exception cref="FieldNotFoundException"></exception>
        public ContextedFieldInfo Field(string name)
        {
            CC.Guard.StringArgument(name, nameof(name));

            if (TypeCache.Fields.TryGetValue(
                    new FieldKey(InstanceType, name),
                    out FieldInfo found)
                    )
                return new ContextedFieldInfo(Instance, found);

            IEnumerable<FieldInfo> fields = IncludeNonPublic ? AllFields.Value : PublicFields.Value;

            found = fields.FirstOrDefault(x => x.Name.EqualsOrdinal(name))
                           ??
                           throw new FieldNotFoundException(InstanceType, name, GetBindingFlags());

            if (!DisallowCaching)
                found.TryCacheMember();

            return new ContextedFieldInfo(Instance, found);


        }
        public ContextedFieldInfo<T> Field<T>(string name)
        {
            return Field(name).Convert<T>();
        }
        public ContextedFieldInfo Field(Type type)
        {
            CC.Guard.NullArgument(type, nameof(type));

            if (TypeCache.Fields.TryGetValue(
                    new FieldKey(InstanceType, type),
                    out FieldInfo found)
                    )
                return new ContextedFieldInfo(Instance, found);

            IEnumerable<FieldInfo> fields = IncludeNonPublic ? AllFields.Value : PublicFields.Value;

            FieldInfo[] filteredFields = (from field in fields
                                          where field.FieldType.IsType(type)
                                          select field).ToArray();

            if (filteredFields.Length > 1
                &&
                fields.FirstOrDefault(x => x.FieldType == type) is FieldInfo preciseMatch
                )
                return new ContextedFieldInfo(Instance, preciseMatch);

            found = fields.FirstOrDefault()
                    ??
                    throw new FieldNotFoundException(InstanceType, type, GetBindingFlags());

            if (!DisallowCaching)
                found.TryCacheMember();

            return new ContextedFieldInfo(Instance, found);
        }
        public ContextedFieldInfo<T> Field<T>() => Field(typeof(T)).Convert<T>();

        /// <exception cref="PropertyNotFoundException"></exception>
        public ContextedPropertyInfo Property(string name)
        {
            CC.Guard.StringArgument(name, nameof(name));

            if (TypeCache.Properties.TryGetValue(
                    new FieldKey(InstanceType, name),
                    out PropertyInfo found)
                    )
                return new ContextedPropertyInfo(Instance, found);

            IEnumerable<PropertyInfo> props = IncludeNonPublic
                                              ? 
                                              AllProperties.Value
                                              : 
                                              PublicProperties.Value;

            found = props.FirstOrDefault(x => x.Name.EqualsOrdinal(name))
                    ??
                    throw new PropertyNotFoundException(InstanceType, name, GetBindingFlags());

            if (!DisallowCaching)
                found.TryCacheMember();

            return new ContextedPropertyInfo(Instance, found);
        }
        public ContextedPropertyInfo<T> Property<T>(string name)
        {
            return Property(name).Convert<T>();
        }
        public ContextedPropertyInfo Property(Type type)
        {
            CC.Guard.NullArgument(type, nameof(type));

            if (TypeCache.Properties.TryGetValue(
                    new FieldKey(InstanceType, type),
                    out PropertyInfo found)
                    )
                return new ContextedPropertyInfo(Instance, found);

            IEnumerable<PropertyInfo> props = IncludeNonPublic
                                              ?
                                              AllProperties.Value
                                              :
                                              PublicProperties.Value;

            props = props.Where(prop => prop.PropertyType.IsType(type)).ToArray();

            if (props.FirstOrDefault(x => x.PropertyType == type) is PropertyInfo preciseMatch)
                return new ContextedPropertyInfo(Instance, preciseMatch);

            found = props.FirstOrDefault()
                    ??
                    throw new PropertyNotFoundException(InstanceType, type, GetBindingFlags());

            if (!DisallowCaching)
                found.TryCacheMember();

            return new ContextedPropertyInfo(Instance, found);
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
            CC.Guard.StringArgument(name, nameof(name));

            if (TypeCache.Methods.TryGetValue(
                new MethodKey(InstanceType, (CCParameters)args, args.GetParameterModifiers()),
                out MethodInfo found)
                )
                return new ContextedMethodInfo(Instance, found);

            IEnumerable<MethodInfo> methods = IncludeNonPublic
                                              ?
                                              AllMethods.Value
                                              :
                                              PublicMethods.Value;

            found = (from method in methods
                     where method.Name.EqualsOrdinal(name)
                     where method.GetCCParameters(ignoreOptionalParameters) == ((CCParameters)args)
                     select method)
                     .FirstOrDefault()
                     ??
                     throw new MethodNotFoundException(InstanceType, name, GetBindingFlags());

            if (!DisallowCaching)
                found.TryCacheMember();

            return new ContextedMethodInfo(Instance, found);
        }

        public ContextedEventInfo Event(string name)
        {
            CC.Guard.StringArgument(name, nameof(name));

            IEnumerable<EventInfo> events = IncludeNonPublic
                                            ?
                                            AllEvents.Value
                                            :
                                            PublicEvents.Value;

            return new ContextedEventInfo(Instance, 
                   events.FirstOrDefault(x => x.Name.EqualsOrdinal(name))
                   ??
                   throw new EventNotFoundException(InstanceType, name, GetBindingFlags()));
        }
        public ContextedEventInfo<T> Event<T>(string name)
            where T : Delegate
        {
            return Event(name).Convert<T>();
        }

        public object TransformType(Type toType)
        {
            CC.Guard.NullArgument(toType, nameof(toType));

            if (Instance is null)
                CC.Throw.InvalidCast(toType, "Cannot not convert null object.");

            Instance = TypeMutator.MutateType(Instance, toType);

            if (Instance is null)
                throw new MissingDataException(nameof(Instance));

            return Instance;
        }
        [DebuggerStepThrough]
        public T TransformType<T>() => (T)TransformType(typeof(T));

        /// <summary>
        /// Cast instance object in specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? As<T>() => (T?)Instance;

        public void CopyTypeDataTo(Reflected reflected)
        {
            foreach (var field in (IncludeNonPublic ? AllFields.Value : PublicFields.Value).Where(field => !field.IsInitOnly))
                reflected.Field(field.Name).SetValue(field.GetValue(Instance));
        }
        public void CopyTypeDataTo(Reflected reflected, params string[] fieldOrPropNames)
        {
            (from field in (IncludeNonPublic ? AllFields.Value : PublicFields.Value)
             where !field.IsInitOnly
             where fieldOrPropNames.Contains(field.Name)
             select field)
             .CForEach(field => reflected.Field(field.Name).SetValue(field.GetValue(Instance)));

            (from prop in (IncludeNonPublic ? AllProperties.Value : PublicProperties.Value)
             where prop.SetMethod is not null
             where fieldOrPropNames.Contains(prop.Name)
             select prop)
             .CForEach(prop => reflected.Property(prop.Name).SetValue(prop.GetValue(Instance)));
        }
        public void CopyTypeDataTo(object otherInstance)
        {
            CopyTypeDataTo(otherInstance.AsReflected());
        }
        public void CopyTypeDataTo(object otherInstance, params string[] fieldOrPropNames)
        {
            CopyTypeDataTo(otherInstance.AsReflected(), fieldOrPropNames);
        }

        public bool Equals(Reflected? other)
        {
            if (other is null)
                return false;

            return Equals(InstanceType, other.InstanceType)
                   &&
                   Equals(Instance, other.Instance);
        }

        public override bool Equals(object obj) => Equals(obj as Reflected);

        public override int GetHashCode() => HashCode.Combine(Instance, InstanceType);

        private BindingFlags GetBindingFlags()
        {
            if (Instance is null
                && 
                settings.IsFlagSetted(Settings.OnlyStaticWhenTargetIsNull)
                )
                return IncludeNonPublic ? StaticAll : StaticPublic;

            return IncludeNonPublic ? All : AllPublic;
        }
    }

    public class Reflected<T> : Reflected
    {
        public Reflected() : base(typeof(T))
        {
        }

        public Reflected(object? instance) : base(typeof(T), instance)
        {
        }

        public Reflected(object? instance, Settings setttings) : base(typeof(T), instance, setttings)
        {
        }
    }
}
