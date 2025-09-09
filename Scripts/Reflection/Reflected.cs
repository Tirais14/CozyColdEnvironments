#nullable enable
using CCEnvs.Cacheables;
using CCEnvs.Common;
using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using CCEnvs.Reflection.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using static CCEnvs.BindingFlagsDefault;

#pragma warning disable IDE1006
namespace CCEnvs.Reflection
{
    /// <summary>
    /// Used for convenient type handling using reflection
    /// </summary>
    public sealed class Reflected : IEquatable<Reflected>
    {
        [Flags]
        public enum Settings 
        {
            Default = IncludeNonPublic | OnlyStaticWhenTargetIsNull,
            None = 0,
            IncludeNonPublic = 1,
            OnlyStaticWhenTargetIsNull = 2
        }

        private ReadOnlyCollection<FieldInfo>? allFields;
        private ReadOnlyCollection<PropertyInfo>? allProperties;
        private ReadOnlyCollection<MethodInfo>? allMethods;
        private ReadOnlyCollection<EventInfo>? allEvents;

        private ReadOnlyCollection<FieldInfo>? publicFields;
        private ReadOnlyCollection<PropertyInfo>? publicProperties;
        private ReadOnlyCollection<MethodInfo>? publicMethods;
        private ReadOnlyCollection<EventInfo>? publicEvents;

        private ReadOnlyCollection<FieldInfo>? nonPublicFields;
        private ReadOnlyCollection<PropertyInfo>? nonPublicProperties;
        private ReadOnlyCollection<MethodInfo>? nonPublicMethods;
        private ReadOnlyCollection<EventInfo>? nonPublicEvents;

        private object? target;

        public ReadOnlyCollection<FieldInfo> AllFields {
            get
            {
                allFields ??= new ReadOnlyCollection<FieldInfo>(
                    TargetType.ForceGetFields(GetBindingFlags()));

                return allFields;
            }
        }

        public ReadOnlyCollection<PropertyInfo> AllProperties {
            get
            {
                allProperties ??= new ReadOnlyCollection<PropertyInfo>(
                    TargetType.ForceGetProperties(GetBindingFlags()));

                return allProperties;
            }
        }

        public ReadOnlyCollection<MethodInfo> AllMethods {
            get
            {
                allMethods ??= new ReadOnlyCollection<MethodInfo>(
                    TargetType.ForceGetMethods(GetBindingFlags()));

                return allMethods;
            }
        }

        public ReadOnlyCollection<EventInfo> AllEvents {
            get
            {
                allEvents ??= new ReadOnlyCollection<EventInfo>(
                    TargetType.ForceGetMembers<EventInfo>(GetBindingFlags()));

                return allEvents;
            }
        }

        public ReadOnlyCollection<FieldInfo> PublicFields {
            get
            {
                publicFields ??= AllFields.Where(x => x.IsPublic).ToReadOnlyCollection();

                return publicFields;
            }
        }

        public ReadOnlyCollection<PropertyInfo> PublicProperties {
            get
            {
                publicProperties ??= AllProperties.Where(x => x.GetAccessors().Any(x => x.IsPublic)).ToReadOnlyCollection();

                return publicProperties;
            }
        }

        public ReadOnlyCollection<MethodInfo> PublicMethods {
            get
            {
                publicMethods ??= AllMethods.Where(x => x.IsPublic).ToReadOnlyCollection();

                return publicMethods;
            }
        }

        public ReadOnlyCollection<EventInfo> PublicEvents {
            get
            {
                publicEvents ??= AllEvents.Where(x => x.AddMethod.IsPublic || x.RemoveMethod.IsPublic).ToReadOnlyCollection();

                return publicEvents;
            }
        }

        public ReadOnlyCollection<FieldInfo> NonPublicFields {
            get
            {
                nonPublicFields ??= AllFields.Where(x => !x.IsPublic).ToReadOnlyCollection();

                return nonPublicFields;
            }
        }

        public ReadOnlyCollection<PropertyInfo> NonPublicProperties {
            get
            {
                nonPublicProperties ??= AllProperties.Where(x => x.GetAccessors().All(x => !x.IsPublic)).ToReadOnlyCollection();

                return nonPublicProperties;
            }
        }

        public ReadOnlyCollection<MethodInfo> NonPublicMethods {
            get
            {
                nonPublicMethods ??= AllMethods.Where(x => !x.IsPublic).ToReadOnlyCollection();

                return nonPublicMethods;
            }
        }

        public ReadOnlyCollection<EventInfo> NonPublicEvents {
            get
            {
                nonPublicEvents ??= AllEvents.Where(x => !x.AddMethod.IsPublic && !x.RemoveMethod.IsPublic).ToReadOnlyCollection();

                return nonPublicEvents;
            }
        }

        public object? Target {
            get => target;
            set
            {
                target = value;
                TargetType = target!.GetType();
            }
        }
        public Type TargetType { get; private set; }
        public Settings settings { get; }
        public bool IncludeNonPublic { get; }

        public Reflected(Type targetType, Settings settings = default)
        {
            target = null;
            TargetType = targetType;
            this.settings = settings;
            IncludeNonPublic = settings.IsFlagSetted(Settings.IncludeNonPublic);
        }

        public Reflected(object target, Settings settings = default)
        {
            this.target = target;
            TargetType = target.GetType();
            this.settings = settings;
            IncludeNonPublic = settings.IsFlagSetted(Settings.IncludeNonPublic);
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
            CC.Validate.StringArgument(nameof(name), name);

            IEnumerable<FieldInfo> fields = IncludeNonPublic ? AllFields : PublicFields;

            FieldInfo found = fields.FirstOrDefault(x => x.Name.EqualsOrdinal(name))
                              ??
                              throw new FieldNotFoundException(TargetType, name, GetBindingFlags());

            return new ContextedFieldInfo(Target, found);
        }
        public ContextedFieldInfo Field(Type type)
        {
            CC.Validate.ArgumentNull(type, nameof(type));

            if (TypeCache.Fields.TryGetValue(
                new FieldKey(TargetType, type),
                out FieldInfo found)
                )
                return new ContextedFieldInfo(Target, found);

            IEnumerable<FieldInfo> fields = IncludeNonPublic ? AllFields : PublicFields;

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
                                              AllProperties
                                              : 
                                              PublicProperties;

            found = (props.FirstOrDefault(x => x.Name.EqualsOrdinal(name))
                    ??
                    throw new PropertyNotFoundException(TargetType, name, GetBindingFlags()))
                    .TryCacheMember();

            return new ContextedPropertyInfo(Target, found);
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
                                              AllProperties
                                              :
                                              PublicProperties;

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

        /// <exception cref="MethodNotFoundException"></exception>
        public ContextedMethodInfo Method(string name,
                                 ExplicitArguments args = default,
                                 bool ignoreOptionalParameters = false)
        {
            CC.Validate.StringArgument(nameof(name), name);

            if (TypeCache.Methods.TryGetValue(
                new MethodKey(TargetType, (CCParameters)args, args.GetParameterModifiers()),
                out MethodInfo found)
                )
                return new ContextedMethodInfo(Target, found);

            IEnumerable<MethodInfo> methods = IncludeNonPublic
                                              ?
                                              AllMethods
                                              :
                                              PublicMethods;

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

        public EventInfo Event(string name)
        {
            CC.Validate.StringArgument(name, nameof(name));

            IEnumerable<EventInfo> events = IncludeNonPublic
                                            ?
                                            AllEvents
                                            :
                                            PublicEvents;

            return events.FirstOrDefault(x => x.Name.EqualsOrdinal(name))
                   ?? 
                   throw new EventNotFoundException(TargetType, name, GetBindingFlags());
        }

        public object ChangeType(Type toType)
        {
            CC.Validate.ArgumentNull(toType, nameof(toType));

            if (Target is null)
                CC.Throw.InvalidCast(toType, "Cannot not convert null object.");

            Target = CCConvert.ChangeType(Target, toType);

            if (Target is null)
                throw new DataAccessException(nameof(Target));

            return Target;
        }
        [DebuggerStepThrough]
        public T ChangeType<T>() => (T)ChangeType(typeof(T));

        public T As<T>()
        {
            if (Target is null)
                CC.Throw.InvalidCast(typeof(void), typeof(T));

            return (T)Target;
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
}
