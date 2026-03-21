using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CCEnvs.Caching;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CommunityToolkit.Diagnostics;
using Humanizer;
using BindingFlags = System.Reflection.BindingFlags;

#nullable enable
#pragma warning disable S3011
namespace CCEnvs.Reflection
{
    //TODO: Caching, expression trees
    public record Reflect : IShallowCloneable<Reflect>
    {
        private readonly static Cache<Type, Type[]> cache = new()
        {
            ExpirationScanFrequency = 30.Seconds()
        };

        [Flags]
        public enum Settings
        {
            None,
            IncludeBaseTypes = 1,
            ByFullName = 1 << 1,
        }

        public Settings settings { get; set; }
        public BindingFlags bindingFlags { get; set; }
        public TypeMatchingSettings typeMatchingSettings { get; set; }
        public MemberTypes? memberTypes { get; set; }
        public Maybe<object> target { get; set; }
        public MemberInfo member { get; set; } = null!;
        public Maybe<string> name { get; set; }
        public Maybe<Type[]> argumentTypes { get; set; }
        public Maybe<object?[]> arguments { get; set; }
        public Maybe<Type> extraType { get; set; }
        public Maybe<ParameterModifier[]> parameterModifiers { get; set; }
        public Maybe<Binder> binder { get; set; }
        public Maybe<Type[]> genericTypes { get; set; }
        public Maybe<Type[]> attributes { get; private set; }

        protected Type type => member.CastTo<Type>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect From(object instance)
        {
            Guard.IsNotNull(instance, nameof(instance));

            this.target = instance;
            member = instance.GetType();

            return IncludeInstance().IncludeStatic().IncludePublic();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect From(MemberInfo member)
        {
            Guard.IsNotNull(member, nameof(member));

            target = null;
            this.member = member;

            return IncludeStatic().IncludePublic();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithBindingFlags(BindingFlags bindingFlags = BindingFlags.Default)
        {
            this.bindingFlags = bindingFlags;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect IncludeMemberTypes(MemberTypes? memberTypes = null)
        {
            this.memberTypes = memberTypes;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect IncludeNonPublic(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.NonPublic;
            else
                bindingFlags &= ~BindingFlags.NonPublic;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithName(string? name = null)
        {
            this.name = name;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect IncludePublic(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.Public;
            else
                bindingFlags &= ~BindingFlags.Public;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect IncludeStatic(bool state = true)
        {
            if (state)
                bindingFlags |= BindingFlags.Static;
            else
                bindingFlags &= ~BindingFlags.Static;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect IncludeInstance(bool state = true)
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
        public Reflect WithBinder(Binder? binder = null)
        {
            this.binder = binder;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithArgumentTypes(params Type[] types)
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
        public Reflect WithArgumentTypes<T>()
        {
            return WithArgumentTypes(typeof(T));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithArgumentTypes<T, T1>()
        {
            return WithArgumentTypes(typeof(T), typeof(T1));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithArgumentTypes<T, T1, T2>()
        {
            return WithArgumentTypes(
                typeof(T),
                typeof(T1),
                typeof(T2)
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithArgumentTypes<T, T1, T2, T3>()
        {
            return WithArgumentTypes(
                typeof(T),
                typeof(T1),
                typeof(T2),
                typeof(T3)
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithArgumentTypes<T, T1, T2, T3, T4>()
        {
            return WithArgumentTypes(
                typeof(T),
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4)
                );
        }

        /// <summary>
        /// Arguments can be null, if types setted by <see cref="WithArgumentTypes(Type[])"/>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithArguments(params object[] args)
        {
            arguments = args;

            if (argumentTypes.IsNone)
            {
                if (args.Any(x => x.IsNull()))
                    ThrowHelper.ThrowArgumentNullException(nameof(args), "Array contains null");

                argumentTypes = args.Select(x => x.GetType()).ToArray();
            }

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithArguments(params TypeValuePair[] pairs)
        {
            arguments = pairs.Select(x => x.Value).ToArray();
            argumentTypes = pairs.Select(x => x.Type).ToArray();

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithoutArguments()
        {
            return WithArguments(Array.Empty<object>());
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithParameterModifiers(ParameterModifier parameterModifier = default)
        {
            parameterModifiers = Range.From(parameterModifier);

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithParameterModifiers(params bool[] flags)
        {
            var paramMods = new ParameterModifier(flags.Length);

            for (int i = 0; i < flags.Length; i++)
                paramMods[i] = flags[i];

            WithParameterModifiers(paramMods);

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithParameterModifiers(ParameterModifier[]? parameterModifiers = null)
        {
            this.parameterModifiers = parameterModifiers;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithTypeFilter(Type? type = null)
        {
            extraType = type;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithTypeFilter<T>()
        {
            return WithTypeFilter(typeof(T));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect WithGenericArguments(params Type[] types)
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
        public Reflect WithDefinedAttributes(params Type[] types)
        {
            attributes = types;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reflect Reset()
        {
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
        public IEnumerable<MemberInfo> Members() => FindMembers();

        public Result<MemberInfo> Member()
        {
            return (Members().FirstOrDefault(), CC.ThrowHelper.MemberNotFoundException(
                name: name.Raw,
                memberType: MemberTypes.Custom,
                reflectedType: type,
                bindingFlags: bindingFlags,
                argumentTypes: argumentTypes.Raw,
                binder: binder.Raw));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<ConstructorInfo> Constructors()
        {
            return IncludeInstance()
                .IncludeMemberTypes(MemberTypes.Constructor)
                .FindMembers()
                .CastCustom<ConstructorInfo>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<ConstructorInfo> Constructor()
        {
            return (Constructors().FirstOrDefault(), CC.ThrowHelper.MemberNotFoundException(
                name: name.Raw,
                memberType: MemberTypes.Constructor,
                reflectedType: type,
                bindingFlags: bindingFlags,
                argumentTypes: argumentTypes.Raw,
                binder: binder.Raw));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<ValuedMemberInfo> ValuedMembers()
        {
            return IncludeMemberTypes().IncludeMemberTypes(MemberTypes.Field | MemberTypes.Property).
                FindMembers()
                .Select(member =>
                {
                    if (member is FieldInfo field)
                        return new ValuedMemberInfo(field);
                    else if (member is PropertyInfo prop)
                        return new ValuedMemberInfo(prop);

                    return null!;
                })
                .Where(x => x.IsNotNull());
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<ValuedMemberInfo> ValuedMember()
        {
            return (ValuedMembers().FirstOrDefault(), CC.ThrowHelper.MemberNotFoundException(
                name: name.Raw,
                memberType: MemberTypes.Field | MemberTypes.Property,
                reflectedType: type,
                bindingFlags: bindingFlags,
                argumentTypes: argumentTypes.Raw,
                binder: binder.Raw));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<FieldInfo> Fields()
        {
            return IncludeMemberTypes().IncludeMemberTypes(MemberTypes.Field).FindMembers().CastCustom<FieldInfo>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<FieldInfo> Field()
        {
            if (name.IsNone && extraType.IsNone)
                throw new InvalidOperationException($"{nameof(name)} and/or {nameof(extraType)} must be setted.");

            return (Fields().FirstOrDefault(), CC.ThrowHelper.MemberNotFoundException(
                name: name.Raw,
                memberType: MemberTypes.Field,
                reflectedType: type,
                bindingFlags: bindingFlags,
                argumentTypes: argumentTypes.Raw,
                binder: binder.Raw));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<PropertyInfo> Properties()
        {
            return IncludeMemberTypes().IncludeMemberTypes(MemberTypes.Property).FindMembers().CastCustom<PropertyInfo>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<PropertyInfo> Property()
        {
            if (name.IsNone && extraType.IsNone)
                throw new InvalidOperationException($"{nameof(name)} and/or {nameof(extraType)} must be setted.");

            return (Properties().FirstOrDefault(), CC.ThrowHelper.MemberNotFoundException(
                name: name.Raw,
                memberType: MemberTypes.Property,
                reflectedType: type,
                bindingFlags: bindingFlags,
                argumentTypes: argumentTypes.Raw,
                binder: binder.Raw));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<MethodInfo> Methods()
        {
            var t = IncludeMemberTypes().IncludeMemberTypes(MemberTypes.Method)
                .FindMembers()
                .CastCustom<MethodInfo>();

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
            return (Methods().FirstOrDefault(), CC.ThrowHelper.MemberNotFoundException(
                name: name.Raw,
                memberType: MemberTypes.Method,
                reflectedType: type,
                bindingFlags: bindingFlags,
                argumentTypes: argumentTypes.Raw,
                binder: binder.Raw));
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
        public TReturn InvokeMethod<TReturn>() => WithTypeFilter<TReturn>().InvokeMethod().CastTo<TReturn>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object CreateInstance()
        {
            ConstructorInfo ctor = IncludeInstance().Constructor().Strict();

            var result = ctor.Invoke(arguments.GetValue(Type.EmptyTypes));

            if (result.IsNull())
                throw new InvalidOperationException("Error while invoking constructor");

            PrintMethodInvoked(ctor);

            return result;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCasted CreateInstance<TCasted>()
        {
            return CreateInstance().CastTo<TCasted>();
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
            return WithTypeFilter<T>().GetFieldValue().Cast<T>().RightTarget.CastTo<T>();
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
            return WithTypeFilter<T>().GetFieldValues().Cast<T>();
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
            return WithTypeFilter<T>().GetPropertyValue().Cast<T>().GetValueUnsafe<T>();
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
            return WithTypeFilter<T>().GetPropertyValues().Cast<T>();
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
        private IEnumerable<MemberInfo> FindMembers()
        {
            if (memberTypes is null)
                return Enumerable.Empty<Type>();

            IReadOnlyCollection<Type> types;
            if (settings.IsFlagSetted(Settings.IncludeBaseTypes))
                types = GetBaseTypes();
            else
                types = Range.From(type);

            var memberTypeArray = memberTypes.Value.ToArrayByFlags();
            IEnumerable<MemberInfo> results = Enumerable.Empty<MemberInfo>();
            IEnumerable<MemberInfo> current;
            foreach (var type in types)
            {
                foreach (var memberType in memberTypeArray)
                {
                    current = memberType switch
                    {
                        MemberTypes.All => throw new NotImplementedException(memberTypes.ToString()),

                        MemberTypes.Constructor => type.GetConstructors(bindingFlags).Where(CompareConstructor),

                        MemberTypes.Custom => throw new NotImplementedException(memberTypes.ToString()),

                        MemberTypes.Event => throw new NotImplementedException(memberTypes.ToString()),

                        MemberTypes.Field => type.GetFields(bindingFlags)
                        .Where(field => CompareValuedMember(field)),

                        MemberTypes.Method => type.GetMethods(bindingFlags).Where(CompareMethod),

                        MemberTypes.NestedType => throw new NotImplementedException(memberTypes.ToString()),

                        MemberTypes.Property => type.GetProperties(bindingFlags)
                        .Where(prop => CompareValuedMember(prop)),

                        MemberTypes.TypeInfo => throw new NotImplementedException(memberTypes.ToString()),

                        _ => throw new InvalidOperationException(memberTypes.ToString()),
                    };

                    results = results.Concat(current);
                }
            }

            return results;
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

            if (!right.TryGetValue(out var r))
                return true;

            return CompareType(left, r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CompareName(string other)
        {
            if (!this.name.TryGetValue(out var name))
                return true;

            if (settings.IsFlagSetted(Settings.ByFullName))
            {
                return other.EqualsOrdinal(name,
                    bindingFlags.IsFlagSetted(BindingFlags.IgnoreCase));
            }

            return other.ContainsOrdinal(name,
                bindingFlags.IsFlagSetted(BindingFlags.IgnoreCase));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CompareAttributes(MemberInfo member)
        {
            if (!this.attributes.TryGetValue(out var attributes))
                return true;

            return attributes.All(type => member.IsDefined(type));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CompareValuedMember(ValuedMemberInfo member)
        {
            if (!CompareName(member.Name))
                return false;
            if (!CompareAttributes(member.Member))
                return false;
            if (!CompareType(member.UnderlyingType, extraType, useFirstArgument: true))
                return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CompareConstructor(ConstructorInfo ctor)
        {
            var parameters = ctor.GetParameters();
            ParameterModifier paramMods = parameters.GetParameterModifiers();

            if (!CompareAttributes(ctor))
                return false;

            if (argumentTypes.IsSome
                &&
                !ctor.CompareParameters(argumentTypes.GetValue(Type.EmptyTypes), paramMods))
            {
                return false;
            }

            return true;
        }

        private void PrintMethodInvoked(MethodBase methodBase)
        {
            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"{(methodBase is ConstructorInfo ? "Constructor" : "Method")} invoked: {methodBase}, argumentsTypes: {argumentTypes.Map(types => types.Select(x => x.GetFullName()).JoinStringsByComma())}, genericArguments: {genericTypes.Map(types => types.Select(x => x.GetFullName()).JoinStringsByComma()).GetValue(string.Empty)}.");
        }

        private IReadOnlyCollection<Type> GetBaseTypes()
        {
            var key = $"{type}, {nameof(GetBaseTypes)}";

            if (!cache.TryGetValue(type, out var baseTypes))
            {
                baseTypes = type.CollectBaseTypes().ToArray();

                if (cache.TryAdd(type, baseTypes, out var entry))
                    entry.ExpirationTimeRelativeToNow = 10.Minutes();
            }

            return baseTypes;
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
