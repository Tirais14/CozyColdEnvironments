using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BindingFlags = System.Reflection.BindingFlags;

#nullable enable
#pragma warning disable S3011
namespace CCEnvs.Reflection
{
    //TODO: Caching, expression trees
    public record Reflect : IShallowCloneable<Reflect>
    {
        [Flags]
        public enum Settings
        {
            None,
            IncludeBaseTypes = 1,
            ByFullName = 2,
            ForceConstructors = 4,
            ForceMethods = 8,
        }

        public readonly static Reflect self = new();
        private Maybe<Type[]> cachedBaseTypes;

        public Settings settings { get; private set; }
        public BindingFlags bindingFlags { get; private set; }
        public TypeMatchingSettings typeMatchingSettings { get; private set; }
        public Maybe<object> target { get; private set; }
        public MemberInfo member { get; private set; } = null!;
        public Maybe<string> name { get; private set; }
        public Maybe<Type[]> argumentTypes { get; private set; }
        public Maybe<object?[]> arguments { get; private set; }
        public Maybe<Type> extraType { get; private set; }
        public Maybe<ParameterModifier[]> parameterModifiers { get; private set; }
        public Maybe<Binder> binder { get; private set; }
        public Maybe<Type[]> genericTypes { get; private set; }
        public Type[] baseTypes {
            get
            {
                if (cachedBaseTypes.IsNone && member is Type t)
                    cachedBaseTypes = t.CollectBaseTypes().ToArray();

                return cachedBaseTypes.GetValue(Type.EmptyTypes);
            }
        }
        public Maybe<Type[]> attributes { get; private set; }

        protected Type type => member.As<Type>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect From(object instance)
        {
            Guard.IsNotNull(instance, nameof(instance));

            this.target = instance;
            member = instance.GetType();

            return Instance().Static().Public();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect From(MemberInfo member)
        {
            Guard.IsNotNull(member, nameof(member));

            target = null;
            this.member = member;

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

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        public Reflect ForceMethods(bool state = true)
        {
            if (state)
                settings |= Settings.ForceMethods;
            else
                settings &= ~Settings.ForceMethods;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect MatchTypesByBaseGenericTypeDefinition(bool state = true)
        {
            if (state)
                typeMatchingSettings |= TypeMatchingSettings.ByBaseGenericTypeDefinition;
            else
                typeMatchingSettings &= ~TypeMatchingSettings.ByBaseGenericTypeDefinition;

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

            arguments = types.Select(type =>
            {
                if (type.IsValueType)
                    return Activator.CreateInstance(type);
                else return null;

            }).ToArray();

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect ArgumentTypes<T>()
        {
            return ArgumentTypes(typeof(T));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect ArgumentTypes<T, T1>()
        {
            return ArgumentTypes(typeof(T), typeof(T1));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect ArgumentTypes<T, T1, T2>()
        {
            return ArgumentTypes(
                typeof(T),
                typeof(T1),
                typeof(T2)
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect ArgumentTypes<T, T1, T2, T3>()
        {
            return ArgumentTypes(
                typeof(T),
                typeof(T1),
                typeof(T2),
                typeof(T3)
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect ArgumentTypes<T, T1, T2, T3, T4>()
        {
            return ArgumentTypes(
                typeof(T),
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4)
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect Arguments(params object[] args)
        {
            if (args.Any(x => x.IsNull()))
                ThrowHelper.ThrowArgumentNullException(nameof(args), "Array contains null");

            arguments = args;
            argumentTypes = args.Select(x => x.GetType()).ToArray();

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect Arguments(params TypeValuePair[] pairs)
        {
            arguments = pairs.Select(x => x.Value).ToArray();
            argumentTypes = pairs.Select(x => x.Type).ToArray();

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
        public Reflect TypeFilter(Type? type = null)
        {
            extraType = type;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect TypeFilter<T>()
        {
            return TypeFilter(typeof(T));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect GenericArguments(params Type[] types)
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
            bindingFlags = default;
            typeMatchingSettings = default;
            target = null;
            member = null!;
            name = null;
            argumentTypes = null;
            arguments = null;
            extraType = null;
            parameterModifiers = null;
            binder = null;
            genericTypes = null;
            attributes = null;

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
            return (Constructors().FirstOrDefault(), GetMemberNotFoundException(MemberTypes.Constructor));
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
                .GetValue(t);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<MethodInfo> Method()
        {
            return (Methods().FirstOrDefault(), GetMemberNotFoundException(MemberTypes.Method));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<object> InvokeMethod()
        {
            MethodInfo method = Method().Strict();

            var result = method.Invoke(target.GetValue(), arguments.GetValue(Type.EmptyTypes));

            PrintMethodInvoked(method);

            return result;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T InvokeMethod<T>() => TypeFilter<T>().InvokeMethod().As<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object InvokeConstructor()
        {
            ConstructorInfo ctor = Constructor().Strict();

            var result = ctor.Invoke(arguments.GetValue(Type.EmptyTypes));

            if (result.IsNull())
                throw new CCException("Error while invoking constructor. Result is null.");

            PrintMethodInvoked(ctor);

            return result;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T InvokeConstructor<T>() => InvokeConstructor().As<T>();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object CreateInstance()
        {
            return Instance().Static().NonPublic().InvokeConstructor();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CreateInstance<T>()
        {
            return CreateInstance().As<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<object> GetFieldValue()
        {
            return Field().Strict().GetValue(target.GetValue());
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> GetFieldValue<T>()
        {
            return TypeFilter<T>().GetFieldValue().Cast<T>().RightTarget.As<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<object> GetFieldValues()
        {
            return Fields().Select(field => field.GetValue(target.Raw));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetFieldValues<T>()
        {
            return TypeFilter<T>().GetFieldValues().Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect SetFieldValue()
        {
            Field().Strict().SetValue(target.GetValue(), arguments.Map(x => x.FirstOrDefault()).GetValue());

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<object> GetPropertyValue()
        {
            return Property().Strict().GetValue(target.GetValue());
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> GetPropertyValue<T>()
        {
            return TypeFilter<T>().GetPropertyValue().Cast<T>().AccessUnsafe().As<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<object> GetPropertyValues()
        {
            return Properties().Select(prop => prop.GetValue(target.Raw));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetPropertyValues<T>()
        {
            return TypeFilter<T>().GetPropertyValues().Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect SetPropertyValue()
        {
            Property().Strict().SetValue(target.GetValue(), arguments.Map(x => x.FirstOrDefault()).GetValue());

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect ShallowClone() => new(this);

        /// <summary>
        /// Left is source, Right is clone.
        /// <br/>Fo
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<Reflect, Reflect> Fork() => (this, ShallowClone());

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
                MemberTypes.Constructor => types.SelectMany(type => type.GetConstructors(bindingFlags)).Where(CompareConstructor),
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

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CompareType(Type left, Type right)
        {
            return left.IsType(right, typeMatchingSettings);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CompareType(Type left, Maybe<Type> right, bool useFirstArgument)
        {
            if (right.IsNone && useFirstArgument && arguments.Raw.IsNotNull())
            {
                right = arguments.Map(x => x.FirstOrDefault())
                                 .Map(arg => arg.GetType())
                                 .GetValueUnsafe();
            }

            return right.Match(
                some: right => CompareType(left, right),
                none: () => true).Raw;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CompareAttributes(MemberInfo member)
        {
            return attributes.Match(
                some: types => types.All(type => member.IsDefined(type)),
                none: () => true)
                .Raw;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CompareFieldOrProp(MemberInfo member)
        {
            Maybe<FieldInfo> field = member as FieldInfo;
            Maybe<PropertyInfo> prop = member as PropertyInfo;

            Type underlyingType = field.Map(x => x.FieldType).GetValue(() => prop.GetValueUnsafe().PropertyType);

            if (!CompareName(member.Name))
                return false;
            if (!CompareAttributes(member))
                return false;
            if (!CompareType(underlyingType, extraType, useFirstArgument: true))
                return false;

            return true;
        }

        private bool CompareMethod(MethodInfo method)
        {
            var parameters = method.GetParameters();
            ParameterModifier paramMods = parameters.GetParameterModifiers();
            var argumentTypes = this.argumentTypes.GetValue(Type.EmptyTypes);

            if (!CompareName(method.Name))
                return false;
            if (!CompareAttributes(method))
                return false;
            if (!method.CompareParameters(argumentTypes, paramMods))
                return false;
            if (!CompareType(method.ReturnType, extraType, useFirstArgument: false))
                return false;

            return true;
        }

        private bool CompareConstructor(ConstructorInfo ctor)
        {
            var parameters = ctor.GetParameters();
            ParameterModifier paramMods = parameters.GetParameterModifiers();
            var argumentTypes = this.argumentTypes.GetValue(Type.EmptyTypes);

            if (!CompareAttributes(ctor))
                return false;
            if (!ctor.CompareParameters(argumentTypes, paramMods))
                return false;

            return true;
        }

        private void PrintMethodInvoked(MethodBase methodBase)
        {
            this.PrintLog($"{(methodBase is ConstructorInfo ? "Constructor" : "Method")} invoked: {methodBase}, argumentsTypes: {argumentTypes.Map(types => types.Select(x => x.GetFullName()).JoinStringsByComma())}, genericArguments: {genericTypes.Map(types => types.Select(x => x.GetFullName()).JoinStringsByComma()).GetValue(string.Empty)}.");
        }
    }

    public static class TypeQueryExtensions 
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Reflect Reflect(this MemberInfo source)
        {
            Guard.IsNotNull(source, nameof(source));

            return new Reflect().From(source);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Reflect Reflect(this object source)
        {
            Guard.IsNotNull(source, nameof(source));

            return new Reflect().From(source);
        }
    }
}
