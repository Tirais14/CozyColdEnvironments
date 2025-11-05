using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using CCEnvs.Unity;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using BindingFlags = System.Reflection.BindingFlags;

#nullable enable
#pragma warning disable S3011
namespace CCEnvs.Reflection
{
    //TODO: Caching, expression trees
    public record Reflect
    {
        [Flags]
        public enum Settings
        {
            None,
            IncludeBaseTypes,
            ByFullName,
            ForceConstructors
        }

        public readonly static Reflect self = new();
        private Maybe<Type[]> cachedBaseTypes;

        public Settings settings { get; private set; }
        public Maybe<object> instance { get; private set; }
        public MemberInfo target { get; private set; } = null!;
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
                if (cachedBaseTypes.IsNone && target is Type t)
                    cachedBaseTypes = t.CollectBaseTypes().ToArray();

                return cachedBaseTypes.Access(Type.EmptyTypes);
            }
        }
        public Maybe<Type[]> attributes { get; private set; }

        private Type type => target.As<Type>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect From(object instance)
        {
            Guard.IsNotNull(instance, nameof(instance));

            this.instance = instance;
            target = instance.GetType();

            return Instance().Static().Public();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect From(MemberInfo member)
        {
            Guard.IsNotNull(member, nameof(member));

            instance = null;
            target = member;

            return Static().Public();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect BindingAttributes(BindingFlags bindingFlags)
        {
            this.bindingFlags = bindingFlags;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect NonPublic(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.NonPublic;
            else
                bindingFlags &= ~BindingFlags.NonPublic;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect Name(string? name = null)
        {
            this.name = name;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect Public(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.Public;
            else
                bindingFlags &= ~BindingFlags.Public;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect Static(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.Static;
            else
                bindingFlags &= ~BindingFlags.Static;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect Instance(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.Instance;
            else
                bindingFlags &= ~BindingFlags.Instance;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect IgnoreCase(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.IgnoreCase;
            else
                bindingFlags &= ~BindingFlags.IgnoreCase;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect IncludeBaseTypes(bool state = true)
        {
            if (state)
                settings |= Settings.IncludeBaseTypes;
            else
                settings &= ~Settings.IncludeBaseTypes;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect ByFullName(bool state = true)
        {
            if (state)
                settings |= Settings.ByFullName;
            else
                settings &= ~Settings.ByFullName;

            return this;
        }

        public Reflect ForceConstructors(bool state = true)
        {
            if (state)
                settings |= Settings.ForceConstructors;
            else
                settings &= ~Settings.ForceConstructors;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect Binder(Binder? binder = null)
        {
            this.binder = binder;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect ArgumentTypes(params Type[] types)
        {
            this.argumentTypes = types;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect Arguments(params object[] args)
        {
            arguments = args;
            argumentTypes = args.Select(x => x.GetType()).ToArray();

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect ParameterModifiers(ParameterModifier parameterModifier = default)
        {
            parameterModifiers = Range.From(parameterModifier);

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect ParameterModifiers(ParameterModifier[]? parameterModifiers = null)
        {
            this.parameterModifiers = parameterModifiers;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect ExtraType(Type? type = null)
        {
            extraType = type;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect ExtraType<T>()
        {
            return ExtraType(typeof(T));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect GenericTypes(params Type[] types)
        {
            genericTypes = types;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect Cache(bool state = true)
        {
            //TODO: Caching

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect Attributes(params Type[] types)
        {
            attributes = types;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect Reset()
        {
            cachedBaseTypes = null;

            settings = default;
            instance = null;
            target = null!;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<ConstructorInfo> Constructors()
        {
            return Instance().FindMembers(MemberTypes.Constructor).Cast<ConstructorInfo>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<ConstructorInfo> Constructor()
        {
            return (Instance().Constructors().FirstOrDefault(), GetMemberNotFoundException(MemberTypes.Constructor));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<FieldInfo> Fields()
        {
            return FindMembers(MemberTypes.Field).Cast<FieldInfo>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<FieldInfo> Field()
        {
            if (name.IsNone && extraType.IsNone)
                throw new InvalidOperationException($"{nameof(name)} and/or {nameof(extraType)} must be setted.");

            return (Fields().FirstOrDefault(), GetMemberNotFoundException(MemberTypes.Field));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<PropertyInfo> Properties()
        {
            return FindMembers(MemberTypes.Property).Cast<PropertyInfo>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<PropertyInfo> Property()
        {
            if (name.IsNone && extraType.IsNone)
                throw new InvalidOperationException($"{nameof(name)} and/or {nameof(extraType)} must be setted.");

            return (Properties().FirstOrDefault(), GetMemberNotFoundException(MemberTypes.Property));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<MethodInfo> Methods()
        {
            var t = FindMembers(MemberTypes.Method).Cast<MethodInfo>();

            return genericTypes.Map(
                genericTypes =>
                {
                    return from method in t
                           select method.ContainsGenericParameters
                           ?
                           method.MakeGenericMethod(genericTypes)
                           :
                           method;
                })
                .Access(t);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<MethodInfo> Method()
        {
            if (name.Raw.IsNullOrEmpty())
                throw new InvalidOperationException($"{nameof(name)} must be setted for methods.");

            return (Methods().FirstOrDefault(), GetMemberNotFoundException(MemberTypes.Method));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Invoke()
        {
            var methodBase = name.Match(
                some: _ => Method().Cast<MethodBase>(),
                none: () => Constructor().Cast<MethodBase>())
                .Raw
                .Strict();

            this.PrintLog($"Method will be invoked: {methodBase.Name}, argumentsTypes: {argumentTypes.Map(types => types.Select(x => x.GetFullName()).JoinStringsByComma())}, genericArguments: {genericTypes.Map(types => types.Select(x => x.GetFullName()).JoinStringsByComma()).Access(string.Empty)}.");


            object[] arguments = this.arguments.Access(Array.Empty<object>());
            return methodBase switch
            {
                MethodInfo method => invokeMethod(method),
                ConstructorInfo ctor => ctor.Invoke(arguments),
                _ => throw new InvalidOperationException(methodBase.GetType().GetFullName())
            };

            object invokeMethod(MethodInfo method)
            {
                if (!method.ContainsGenericParameters
                    &&
                    genericTypes.ItIs(x => x.IsNullOrEmpty())
                    )
                    throw new CCException($"Is open generic method, but not found {nameof(genericTypes)}.");

                return method.Invoke(target, arguments);
            }
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Invoke<T>() => Invoke().As<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                MemberTypes.Field => types.SelectMany(type => type.GetFields(bindingFlags)).Where(CompareFieldOrProp),
                MemberTypes.Method => types.SelectMany(type => type.GetMethods(bindingFlags)).Where(CompareMethod),
                MemberTypes.NestedType => throw new NotImplementedException(memberType.ToString()),
                MemberTypes.Property => types.SelectMany(type => type.GetProperties(bindingFlags)).Where(CompareFieldOrProp),
                MemberTypes.TypeInfo => throw new NotImplementedException(memberType.ToString()),
                _ => throw new InvalidOperationException(memberType.ToString()),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CompareFieldOrProp(MemberInfo member)
        {
            Maybe<FieldInfo> field = member as FieldInfo;
            Maybe<PropertyInfo> prop = member as PropertyInfo;

            Type underlyingType = field.Map(x => x.FieldType).Access(() => prop.AccessUnsafe().PropertyType);

            if (!CompareName(member.Name) 
                ||
                (extraType.IsSome
                &&
                underlyingType.IsNotType(extraType.AccessUnsafe()))
                )
                return false;

            return true;
        }

        private 

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

            ParameterModifier mods = parameters.GetParameterModifiers();
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
                (methodTyped.ContainsGenericParameters
                &&
                methodTyped.GetGenericArguments().Length != genericTypes.Access(Type.EmptyTypes).Length)))
                return false;

            return true;
        }
    }

    public static class TypeQueryExtensions 
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Reflect ReflectQuery(this MemberInfo source)
        {
            Guard.IsNotNull(source, nameof(source));

            return Reflection.Reflect.self.Reset().From(source);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Reflect ReflectQuery(this object source)
        {
            Guard.IsNotNull(source, nameof(source));

            return Reflection.Reflect.self.Reset().From(source);
        }
    }
}
