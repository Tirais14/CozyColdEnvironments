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
    //public class TypeQuery
    //{
    //    [Flags]
    //    public enum Settings
    //    {
    //        None,
    //        IncludeBaseTypes,
    //        ByFullName
    //    }

    //    public Maybe<object> instance { get; private set; }
    //    public Type type { get; private set; }
    //    public Maybe<string> name { get; private set; }
    //    public BindingFlags bindingFlags { get; private set; }
    //    public Type[] arguments { get; private set; }
    //    public ParameterModifier[] parameterModifiers { get; private set; }
    //    public Binder? binder { get; private set; }
    //    public Settings settings { get; private set; }

    //    public TypeQuery From(object? instance = null)
    //    {
    //        this.instance = instance;
    //        type = instance?.GetType();

    //        return this;
    //    }

    //    public TypeQuery From(Type type)
    //    {
    //        Guard.IsNotNull(type, nameof(type));
    //        instance = null;
    //        this.type = type;

    //        return this;
    //    }

    //    public TypeQuery NonPublic(bool state = true)
    //    {
    //        if (state)
    //            bindingFlags |= BindingFlags.NonPublic;
    //        else
    //            bindingFlags &= ~BindingFlags.NonPublic;

    //        return this;
    //    }

    //    public TypeQuery Name(string? name = null)
    //    {
    //        this.name = name;

    //        return this;
    //    }

    //    public TypeQuery Public(bool state = true)
    //    {
    //        if (state)
    //            bindingFlags |= BindingFlags.Public;
    //        else
    //            bindingFlags &= ~BindingFlags.Public;

    //        return this;
    //    }

    //    public TypeQuery Static(bool state = true)
    //    {
    //        if (state)
    //            bindingFlags |= BindingFlags.Static;
    //        else
    //            bindingFlags &= ~BindingFlags.Static;

    //        return this;
    //    }

    //    public TypeQuery Instance(bool state = true)
    //    {
    //        if (state)
    //            bindingFlags |= BindingFlags.Instance;
    //        else
    //            bindingFlags &= ~BindingFlags.Instance;

    //        return this;
    //    }

    //    public TypeQuery IgnoreCase(bool state = true)
    //    {
    //        if (state)
    //            bindingFlags |= BindingFlags.IgnoreCase;
    //        else
    //            bindingFlags &= ~BindingFlags.IgnoreCase;

    //        return this;
    //    }

    //    public TypeQuery IncludeBaseTypes(bool state = true)
    //    {
    //        if (state)
    //            settings |= Settings.IncludeBaseTypes;
    //        else
    //            settings &= ~Settings.IncludeBaseTypes;

    //        return this;
    //    }

    //    public TypeQuery ByFullName(bool state = true)
    //    {
    //        if (state)
    //            settings |= Settings.ByFullName;
    //        else
    //            settings &= ~Settings.ByFullName;

    //        return this;
    //    }

    //    public TypeQuery Binder(Binder? binder = null)
    //    {
    //        this.binder = binder;

    //        return this;
    //    }

    //    public TypeQuery Arguments(params Type[] types)
    //    {
    //        arguments = types;

    //        return this;
    //    }

    //    public TypeQuery ParameterModifiers(ParameterModifier parameterModifier)
    //    {
    //        parameterModifiers = Range.From(parameterModifier);

    //        return this;
    //    }

    //    public TypeQuery ParameterModifiers(ParameterModifier[] parameterModifiers)
    //    {
    //        this.parameterModifiers = parameterModifiers;

    //        return this;
    //    }

    //    public IEnumerable<MethodInfo> Methods()
    //    {
    //        from method in type.GetMethods(bindingFlags)
    //        where name.MapUnsafe(n =>
    //        {
    //            if (name.IsNone)
    //                return true;

    //            if (settings.IsFlagSetted(Settings.ByFullName))
    //                return n!.EqualsOrdinal(method.Name,
    //                    bindingFlags.IsFlagSetted(BindingFlags.IgnoreCase));
    //            else
    //                return n!.ContainsOrdinal(method.Name,
    //                    bindingFlags.IsFlagSetted(BindingFlags.IgnoreCase));
    //        }).Target
    //        where method.GetParameters().Select(x => x.ParameterType)
    //    }

    //    public Result<MethodInfo> Method()
    //    {
    //        name.IfSome(name => type.GetMethod(name, bindingFlags, binder, arguments, parameterModifiers)).IfNone(() => type.)
    //    }
    //}
}
