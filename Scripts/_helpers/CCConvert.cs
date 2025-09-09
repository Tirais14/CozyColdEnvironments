#nullable enable
using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace CCEnvs
{
    public static class CCConvert
    {
        public static object ChangeType(object target, Type toType)
        {
            CC.Validate.ArgumentNull(target, nameof(target));
            CC.Validate.ArgumentNull(toType, nameof(toType));

            if (target.IsType(toType))
                return target;

            Type targetType = target.GetType();
            object result = null!;

            if (TryConvertByInterface())
                return result;

            if (TryConvertByOverloadedCastOperator())
                return result;

            if (TryConvertByAttributedMethods())
                return result;

            if (TryConvertByDefaultConverter())
                return result;

            //Until this line converstaion could be done with abstraction
            CC.Validate.Argument(toType, nameof(toType), x => !x.IsAbstract && !x.IsInterface);

            CreateByFactory();

            return result;

            bool TryConvertByAttributedMethods()
            {
                MethodInfo? found = (from method in targetType.ForceGetMethods(BindingFlagsDefault.InstanceAll)
                                     where method.IsDefined<ConverterAttribute>(inherit: true)
                                     where method.ReturnType.IsType(toType)
                                     where method.GetCCParameters(ignoreOptionalParameters: true) == CCParameters.Empty
                                     select method)
                                     .FirstOrDefault();

                if (found is null)
                {
                    var seekingsParams = new CCParameters(new CCParameterInfo(targetType))
                    {
                        IgnoreOptionalInEquals = true
                    };
                    found = (from method in targetType.ForceGetMethods(BindingFlagsDefault.StaticAll)
                             where method.IsDefined<ConverterAttribute>(inherit: true)
                             where method.ReturnType.IsType(toType)
                             where method.GetCCParameters(ignoreOptionalParameters: true) == seekingsParams
                             select method)
                             .FirstOrDefault();

                    result = found.Invoke(null!, CC.Create.Array(target));
                }
                else
                    result = found.Invoke(target, CC.EmptyArguments);

                return result is not null;
            }

            bool TryConvertByOverloadedCastOperator()
            {
                if (TryGetCastOperator(targetType,
                                       toType,
                                       out MethodInfo? conversationOperator))
                {
                    result = conversationOperator.Invoke(null, CC.Create.Array(target));

                    return result is not null;
                }

                return false;
            }

            bool TryConvertByInterface()
            {
                if (target is IConvertibleCC convertible)
                {
                    result = convertible.Convert();

                    return result is not null && result.GetType().IsType(toType);
                }

                return false;
            }

            bool TryConvertByDefaultConverter()
            {
                try
                {
                    result = Convert.ChangeType(target, toType);

                    return result is not null;
                }
                catch (InvalidCastException)
                {
                    return false;
                }
            }

            void CreateByFactory()
            {
                try
                {
                    result = InstanceFactory.Create(toType,
                        new ExplicitArguments(new ExplicitArgument(target)),
                        InstanceFactory.Parameters.CacheConstructor
                        |
                        InstanceFactory.Parameters.ThrowIfNotFound
                        |
                        InstanceFactory.Parameters.NonPublic);
                }
                catch (CCException ex)
                {
                    throw new InvalidCastException($"Cannot convert {targetType.GetName()} to {toType.GetName()}. Object must be implement one of the conversation variants: {nameof(IConvertibleCC)}, {nameof(ConverterAttribute)}, explicit/implicit overloaded cast operator, {nameof(IConvertible)}, constructor of {targetType.GetName()} which input takes in argument {toType.GetName()}.", ex);
                }
            }
        }
        [DebuggerStepThrough]
        public static T ChangeType<T>(object target)
        {
            return (T)ChangeType(target, typeof(T));
        }
        [DebuggerStepThrough]

        /// <exception cref="ArgumentException"></exception>
        public static object ChangeType(ITypeProvider target)
        {
            CC.Validate.ArgumentNull(target, nameof(target));
            if (target.ObjectType is null)
                throw new ArgumentException(nameof(target.ObjectType));

            return ChangeType(target, target.ObjectType);
        }
        [DebuggerStepThrough]
        /// <exception cref="ArgumentException"></exception>
        public static T ChangeType<T>(ITypeProvider target)
        {
            CC.Validate.ArgumentNull(target, nameof(target));
            if (target.ObjectType is null)
                throw new ArgumentException(nameof(target.ObjectType));
            if (target.ObjectType.IsNotType(typeof(T)))
                throw new ArgumentException(nameof(target.ObjectType));

            return (T)ChangeType(target, target.ObjectType);
        }

        private static bool TryGetCastOperator(Type ofType,
            Type toType,
            [NotNullWhen(true)] out MethodInfo? result)
        {
            result = ofType.GetOverloadedCastOperator(toType);

            return result is not null;
        }
    }
}
