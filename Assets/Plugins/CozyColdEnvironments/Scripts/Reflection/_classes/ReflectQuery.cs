using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using CCEnvs.Unity;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BindingFlags = System.Reflection.BindingFlags;

#nullable enable
#pragma warning disable S3011
namespace CCEnvs.Reflection
{
    //TODO: Caching, expression trees
    public record ReflectQuery
    {
        [Flags]
        public enum Settings
        {
            None,
            IncludeBaseTypes,
            ByFullName
        }

        public readonly static ReflectQuery self = new();
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
        public Maybe<Type[]> genericTypes { get; private set; }
        public Type[] baseTypes {
            get
            {
                if (cachedBaseTypes.IsNone && source is Type t)
                    cachedBaseTypes = t.CollectBaseTypes().ToArray();

                return cachedBaseTypes.Access(Type.EmptyTypes);
            }
        }
        public Maybe<Type[]> attributes { get; private set; }

        private Type type => source.As<Type>();

        public ReflectQuery From(object instance)
        {
            Guard.IsNotNull(instance, nameof(instance));

            this.instance = instance;
            source = instance.GetType();

            return Instance().Static().Public();
        }

        public ReflectQuery From(MemberInfo member)
        {
            Guard.IsNotNull(member, nameof(member));

            instance = null;
            source = member;

            return Static().Public();
        }

        public ReflectQuery BindingAttributes(BindingFlags bindingFlags)
        {
            this.bindingFlags = bindingFlags;

            return this;
        }

        public ReflectQuery NonPublic(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.NonPublic;
            else
                bindingFlags &= ~BindingFlags.NonPublic;

            return this;
        }

        public ReflectQuery Name(string? name = null)
        {
            this.name = name;

            return this;
        }

        public ReflectQuery Public(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.Public;
            else
                bindingFlags &= ~BindingFlags.Public;

            return this;
        }

        public ReflectQuery Static(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.Static;
            else
                bindingFlags &= ~BindingFlags.Static;

            return this;
        }

        public ReflectQuery Instance(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.Instance;
            else
                bindingFlags &= ~BindingFlags.Instance;

            return this;
        }

        public ReflectQuery IgnoreCase(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.IgnoreCase;
            else
                bindingFlags &= ~BindingFlags.IgnoreCase;

            return this;
        }

        public ReflectQuery IncludeBaseTypes(bool state = true)
        {
            if (state)
                settings |= Settings.IncludeBaseTypes;
            else
                settings &= ~Settings.IncludeBaseTypes;

            return this;
        }

        public ReflectQuery ByFullName(bool state = true)
        {
            if (state)
                settings |= Settings.ByFullName;
            else
                settings &= ~Settings.ByFullName;

            return this;
        }

        public ReflectQuery Binder(Binder? binder = null)
        {
            this.binder = binder;

            return this;
        }

        public ReflectQuery ArgumentTypes(params Type[] types)
        {
            this.argumentTypes = types;

            return this;
        }

        public ReflectQuery Arguments(params object[] args)
        {
            arguments = args;
            argumentTypes = args.Select(x => x.GetType()).ToArray();

            return this;
        }

        public ReflectQuery ParameterModifiers(ParameterModifier parameterModifier = default)
        {
            parameterModifiers = Range.From(parameterModifier);

            return this;
        }

        public ReflectQuery ParameterModifiers(ParameterModifier[]? parameterModifiers = null)
        {
            this.parameterModifiers = parameterModifiers;

            return this;
        }

        public ReflectQuery ExtraType(Type? type = null)
        {
            extraType = type;

            return this;
        }

        public ReflectQuery ExtraType<T>()
        {
            return ExtraType(typeof(T));
        }

        public ReflectQuery GenericTypes(params Type[] types)
        {
            genericTypes = types;

            return this;
        }

        public ReflectQuery Cache(bool state = true)
        {
            //TODO: Caching

            return this;
        }

        public ReflectQuery Attributes(params Type[] types)
        {
            attributes = types;

            return this;
        }

        public ReflectQuery Reset()
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
            return FindMembers(MemberTypes.Constructor).Cast<ConstructorInfo>();
        }

        public Result<ConstructorInfo> Constructor()
        {
            ConstructorInfo? ctor = type.GetConstructor(
                bindingFlags,
                binder.Raw,
                argumentTypes.Access(Type.EmptyTypes),
                parameterModifiers.Access(Array.Empty<ParameterModifier>())
                );

            if (ctor is null || !CompareMethod(ctor))
                ctor = Constructors().FirstOrDefault();

            return (ctor, GetMemberNotFoundException(MemberTypes.Constructor));
        }

        public IEnumerable<FieldInfo> Fields()
        {
            return FindMembers(MemberTypes.Field).Cast<FieldInfo>();
        }

        public Result<FieldInfo> Field()
        {
            FieldInfo? field = name.Map(name => type.GetField(name, bindingFlags)).Raw;

            if (field is null || !CompareField(field))
            {
                if (name.IsNone && extraType.IsNone)
                    this.PrintError($"{nameof(name)} and/or {nameof(extraType)} must be setted.");
                else
                    field = Fields().FirstOrDefault();
            }

            return (field!, GetMemberNotFoundException(MemberTypes.Field));
        }

        public IEnumerable<PropertyInfo> Properties()
        {
            return FindMembers(MemberTypes.Property).Cast<PropertyInfo>();
        }

        public Result<PropertyInfo> Property()
        {
            PropertyInfo? prop = name.Map(name => type.GetProperty(name, bindingFlags)).Raw;

            if (prop is null || !CompareProp(prop))
            {
                if (name.IsNone && extraType.IsNone)
                    this.PrintError($"{nameof(name)} and/or {nameof(extraType)} must be setted.");
                else
                    prop = Properties().FirstOrDefault();
            }

            return (prop!, GetMemberNotFoundException(MemberTypes.Property));
        }

        public IEnumerable<MethodInfo> Methods()
        {
            return FindMembers(MemberTypes.Method).Cast<MethodInfo>();
        }

        public Result<MethodInfo> Method()
        {
            MethodInfo? method = name.Map(name => type.GetMethod(
                name,
                bindingFlags,
                binder.Raw,
                argumentTypes.Access(Type.EmptyTypes),
                parameterModifiers.Access(Array.Empty<ParameterModifier>())))
                .Raw;

            if (method is null || !CompareMethod(method))
                method = Methods().FirstOrDefault();

            return (method, GetMemberNotFoundException(MemberTypes.Method));
        }

        public Result<object> Invoke()
        {
            var method = name.Match(
                some: _ => Method().Cast<MethodBase>(),
                none: () => Constructor().Cast<MethodBase>())
                .Raw;

            if (method.Raw is null)
                return (null!, method.exception);

            this.PrintLog($"Method will be invoked: {method.Raw.Name}, argCount: {method.Raw.GetParameters().Length}.");

            if (!method.Raw.IsConstructedGenericMethod
                &&
                method.Raw is MethodInfo methodTyped)
            {
                method = (methodTyped.MakeGenericMethod(
                    genericTypes.IfNone(
                        () => throw new CCException($"Is open generic method, but not found {nameof(genericTypes)}."))
                    .Access(Type.EmptyTypes)),
                    method.exception);
            }

            return (method.Raw.Invoke(instance, arguments.Access(Array.Empty<object>())), method.exception);
        }

        public Result<T> Invoke<T>()
        {
            var t = Invoke();

            return (t.Raw.As<T>(), t.exception);
        }

        private IEnumerable<MemberInfo> FindMembers(MemberTypes memberType)
        {
            Type[] types;

            if (settings.IsFlagSetted(Settings.IncludeBaseTypes))
                types = baseTypes;
            else
                types = Range.From(type);

            return memberType switch
            {
                MemberTypes.All => throw new NotImplementedException(memberType.ToString()),
                MemberTypes.Constructor => types.SelectMany(type => type.GetConstructors(bindingFlags)).Where(CompareMethod),
                MemberTypes.Custom => throw new NotImplementedException(memberType.ToString()),
                MemberTypes.Event => throw new NotImplementedException(memberType.ToString()),
                MemberTypes.Field => types.SelectMany(type => type.GetFields(bindingFlags)).Where(CompareField),
                MemberTypes.Method => types.SelectMany(type => type.GetMethods(bindingFlags)).Where(CompareMethod),
                MemberTypes.NestedType => throw new NotImplementedException(memberType.ToString()),
                MemberTypes.Property => types.SelectMany(type => type.GetProperties(bindingFlags)).Where(CompareProp),
                MemberTypes.TypeInfo => throw new NotImplementedException(memberType.ToString()),
                _ => throw new InvalidOperationException(memberType.ToString()),
            };
        }

        private bool CompareName(string other)
        {
            return name.Match(
                some: name =>
                {
                    if (settings.IsFlagSetted(Settings.ByFullName))
                    {
                        return other.EqualsOrdinal(name,
                            bindingFlags.IsFlagSetted(BindingFlags.IgnoreCase));
                    }
                    else
                    {
                        return other.ContainsOrdinal(name,
                            bindingFlags.IsFlagSetted(BindingFlags.IgnoreCase));
                    }
                },
                none: () => true)
                .Raw;
        }

        private MemberNotFoundException GetMemberNotFoundException(MemberTypes memberType)
        {
            return new MemberNotFoundException(
                memberType,
                reflectedType: type,
                name: name.Raw,
                bindingFlags: bindingFlags,
                types: argumentTypes.Raw,
                binder: binder.Raw);
        }

        private bool CompareField(FieldInfo field)
        {
            if (!CompareName(field.Name) 
                ||
                (extraType.IsSome
                && 
                field.FieldType.IsNotType(extraType.AccessUnsafe()))
                )
                return false;

            return true;
        }

        private bool CompareProp(PropertyInfo prop)
        {
            if (!CompareName(prop.Name))
                return false;

            return true;
        }

        private bool CompareMethod(MethodBase method)
        {
            if (method is not ConstructorInfo
                &&
                !CompareName(method.Name)
                )
                return false;

            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length != argumentTypes.Access(Type.EmptyTypes).Length
                ||
                parameters.Zip(argumentTypes.Access(Type.EmptyTypes), (param, other) => param.ParameterType.IsType(other)).Any(x => !x)
                )
                return false;

            ParameterModifier mods = method.GetParameters().GetParameterModifiers();
            for (int i = 0; i < parameters.Length; i++)
            {
                if ((parameterModifiers.IsNone && mods[i])
                    ||
                    (parameterModifiers.IsSome
                    &&
                    parameterModifiers.AccessUnsafe()[0][i] != mods[i])
                    )
                    return false;
            }

            if (method is MethodInfo methodTyped
                &&
                (extraType.IsSome
                && 
                methodTyped.ReturnType.IsNotType(extraType.AccessUnsafe())
                ||
                (genericTypes.Raw.IsNotNullOrEmpty()
                &&
                methodTyped.IsGenericMethod
                &&
                methodTyped.GetGenericArguments().Length != genericTypes.Access(Type.EmptyTypes).Length))
                )
                return false;

            return true;
        }
    }

    public static class TypeQueryExtensions 
    {
        public static ReflectQuery ReflectQuery(this MemberInfo source)
        {
            Guard.IsNotNull(source, nameof(source));

            return Reflection.ReflectQuery.self.Reset().From(source);
        }

        public static ReflectQuery ReflectQuery(this object source)
        {
            Guard.IsNotNull(source, nameof(source));

            return Reflection.ReflectQuery.self.Reset().From(source);
        }
    }
}
