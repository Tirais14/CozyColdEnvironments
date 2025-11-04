using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
#pragma warning disable S3011
namespace CCEnvs.Reflection
{
    public record TypeQuery
    {
        [Flags]
        public enum Settings
        {
            None,
            IncludeBaseTypes,
            ByFullName
        }

        public readonly static TypeQuery self = new();
        private Maybe<Type[]> cachedBaseTypes;

        public Settings settings { get; private set; }
        public Maybe<object> instance { get; private set; }
        public MemberInfo source { get; private set; } = null!;
        public Maybe<string> name { get; private set; }
        public BindingFlags bindingFlags { get; private set; }
        public Maybe<Type[]> argumentTypes { get; private set; }
        public Maybe<object[]> arguments { get; private set; }
        public Maybe<Type> extraType { get; private set; }
        public Maybe<ParameterModifier[]> parameterModifiers { get; private set; }
        public Maybe<Binder> binder { get; private set; }
        public Maybe<Type> genericTypes { get; private set; }
        public Type[] baseTypes {
            get
            {
                if (cachedBaseTypes.IsNone && source is Type t)
                    cachedBaseTypes = t.CollectBaseTypes().ToArray();

                return cachedBaseTypes.Access(Type.EmptyTypes);
            }
        }

        private Type type => source.As<Type>();

        private static bool CompareField(MemberInfo member, object input)
        {
            var query = (TypeQuery)input;
            var field = (FieldInfo)member;

            if (!query.CompareName(field.Name))
                return false;

            return true;
        }

        private static bool CompareMethod(MemberInfo member, object input)
        {
            var query = (TypeQuery)input;
            var method = (MethodBase)member;

            if (method is not ConstructorInfo
                &&
                !query.CompareName(method.Name)
                )
                return false;

            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length != query.argumentTypes.Access(Type.EmptyTypes).Length)
                return false;

            if (!parameters.Select(x => x.ParameterType).SequenceEqual(query.argumentTypes.Access(Type.EmptyTypes)))
                return false;

            ParameterModifier mods = method.GetParameters().GetParameterModifiers();
            for (int i = 0; i < parameters.Length; i++)
            {
                if ((query.parameterModifiers.IsNone && mods[i])
                    ||
                    (query.parameterModifiers.IsSome && query.parameterModifiers.AccessUnsafe()[0][i] != mods[i])
                    )
                    return false;
            }

            if (method is MethodInfo mInfo
                &&
                mInfo.ReturnType != query.extraType.Access(typeof(void)))
                return false;

            return true;
        }

        public TypeQuery From(object instance)
        {
            Guard.IsNotNull(instance, nameof(instance));

            this.instance = instance;
            source = instance.GetType();

            return Instance().Static().Public();
        }

        public TypeQuery From(MemberInfo member)
        {
            Guard.IsNotNull(member, nameof(member));

            instance = null;
            source = member;

            return Static().Public();
        }

        public TypeQuery NonPublic(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.NonPublic;
            else
                bindingFlags &= ~BindingFlags.NonPublic;

            return this;
        }

        public TypeQuery Name(string? name = null)
        {
            this.name = name;

            return this;
        }

        public TypeQuery Public(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.Public;
            else
                bindingFlags &= ~BindingFlags.Public;

            return this;
        }

        public TypeQuery Static(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.Static;
            else
                bindingFlags &= ~BindingFlags.Static;

            return this;
        }

        public TypeQuery Instance(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.Instance;
            else
                bindingFlags &= ~BindingFlags.Instance;

            return this;
        }

        public TypeQuery IgnoreCase(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.IgnoreCase;
            else
                bindingFlags &= ~BindingFlags.IgnoreCase;

            return this;
        }

        public TypeQuery IncludeBaseTypes(bool state = true)
        {
            if (state)
                settings |= Settings.IncludeBaseTypes;
            else
                settings &= ~Settings.IncludeBaseTypes;

            return this;
        }

        public TypeQuery ByFullName(bool state = true)
        {
            if (state)
                settings |= Settings.ByFullName;
            else
                settings &= ~Settings.ByFullName;

            return this;
        }

        public TypeQuery Binder(Binder? binder = null)
        {
            this.binder = binder;

            return this;
        }

        public TypeQuery Arguments(params Type[] types)
        {
            this.argumentTypes = types;

            return this;
        }

        public TypeQuery ParameterModifiers(ParameterModifier parameterModifier = default)
        {
            parameterModifiers = Range.From(parameterModifier);

            return this;
        }

        public TypeQuery ParameterModifiers(ParameterModifier[]? parameterModifiers = null)
        {
            this.parameterModifiers = parameterModifiers;

            return this;
        }

        public TypeQuery Reset()
        {
            cachedBaseTypes = null;

            settings = default;
            instance = null;
            source = null!;
            name = null;
            bindingFlags = default;
            argumentTypes = null;
            arguments = null;
            extraType = null;
            parameterModifiers = null;
            binder = null;
            genericTypes = null;

            return this;
        }

        public IEnumerable<ConstructorInfo> Constructors()
        {
            return FindMembers(MemberTypes.Constructor, CompareMethod).Cast<ConstructorInfo>();
        }

        public Result<ConstructorInfo> Constructor()
        {
            ConstructorInfo? ctor = type.GetConstructor(
                bindingFlags,
                binder.Target,
                argumentTypes.Access(Type.EmptyTypes),
                parameterModifiers.Access(Array.Empty<ParameterModifier>())
                );

            if (ctor is null || !CompareMethod(ctor, this))
                ctor = Constructors().FirstOrDefault();

            return (ctor, new MemberNotFoundException(
                MemberTypes.Constructor,
                reflectedType: type,
                name: name.Target,
                bindingFlags: bindingFlags,
                types: argumentTypes.Access(Type.EmptyTypes),
                binder: binder.Target)
                );
        }

        public IEnumerable<FieldInfo> Fields()
        {
            return FindMembers(MemberTypes.Field, CompareField).Cast<FieldInfo>();
        }

        public Result<FieldInfo> Field()
        {
            FieldInfo? field = name.Map(name => type.GetField(name, bindingFlags))
                .Where(field => extraType.Match(
                    some: extraType => field.FieldType.IsType(extraType),
                    none: () => true).Target)
                .Target;


            if (field is null || !CompareField(field, this))
                field = Fields().FirstOrDefault();

            return (field, new MemberNotFoundException(
                MemberTypes.Field,
                reflectedType: type,
                name: name.Target,
                bindingFlags));
        }

        public IEnumerable<PropertyInfo> Properties()
        {
            return FindMembers(MemberTypes.Property, CompareField).Cast<PropertyInfo>();
        }

        public Result<PropertyInfo> Property()
        {
            PropertyInfo? prop = name.Map(name => type.GetProperty(name, bindingFlags))
                .Where(prop =>
                {
                    return extraType.Match(
                        some: returnType => prop.PropertyType.IsType(returnType),
                        none: () => true)
                    .Target;
                })
                .Target;

            if (prop is null || !CompareField(prop, this))
                prop = Properties().FirstOrDefault();

            return (prop, new MemberNotFoundException(
                MemberTypes.Property,
                reflectedType: type,
                name: name.Target,
                bindingFlags,
                types: argumentTypes.Access(Type.EmptyTypes),
                binder: binder.Target)
                );
        }

        public IEnumerable<MethodInfo> Methods()
        {
            return FindMembers(MemberTypes.Method, CompareMethod).Cast<MethodInfo>();
        }

        public Result<MethodInfo> Method()
        {
            MethodInfo? method = name.Map(name => type.GetMethod(
                name,
                bindingFlags,
                binder.Target,
                argumentTypes.Access(Type.EmptyTypes),
                parameterModifiers.Access(Array.Empty<ParameterModifier>())))
                .Target;

            if (method is null || !CompareMethod(method, this))
                method = Methods().FirstOrDefault();

            return (method, new MemberNotFoundException(
                MemberTypes.Method,
                reflectedType: type,
                name: name.Target,
                bindingFlags: bindingFlags,
                types: argumentTypes.Access(Type.EmptyTypes),
                binder: binder.Target));
        }

        public Result<object> Invoke()
        {
            var method = name.Match(
                some: _ => Method().Cast<MethodBase>(),
                none: () => Constructor().Cast<MethodBase>())
                .Target;

            if (method.Raw is null)
                return (null!, method.exception);

            this.PrintLog($"Method will be invoked: {method.Raw.Name}, argCount: {method.Raw.GetParameters().Length}.");

            try
            {
                return (method.Raw.Invoke(instance, arguments.Access(Array.Empty<object>())), null);
            }
            catch (Exception ex)
            {
                return (null!, ex);
            }
        }

        private IEnumerable<MemberInfo> FindMembers(MemberTypes memberType, MemberFilter filter)
        {
            if (settings.IsFlagSetted(Settings.IncludeBaseTypes))
            {
                return baseTypes.SelectMany(x => x.FindMembers(
                    memberType,
                    bindingFlags,
                    filter,
                    this));
            }
            return type.FindMembers(
                memberType,
                bindingFlags,
                filter,
                this);
        }

        private bool CompareName(string other)
        {
            return name.Match(
                some: name =>
                {
                    if (settings.IsFlagSetted(Settings.ByFullName))
                        return name.EqualsOrdinal(other,
                            bindingFlags.IsFlagSetted(BindingFlags.IgnoreCase));
                    else
                        return name.ContainsOrdinal(other,
                            bindingFlags.IsFlagSetted(BindingFlags.IgnoreCase));
                },
                none: () => false)
                .Target;
        }
    }

    public static class TypeQueryExtensions 
    {
        public static TypeQuery ReflectQuery(this MemberInfo source)
        {
            Guard.IsNotNull(source, nameof(source));

            return TypeQuery.self.Reset().From(source);
        }

        public static TypeQuery ReflectQuery(this object source)
        {
            Guard.IsNotNull(source, nameof(source));

            return TypeQuery.self.Reset().From(source);
        }
    }
}
