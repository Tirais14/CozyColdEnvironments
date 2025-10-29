#nullable enable
using CCEnvs.Reflection;
using System;
using System.Linq;
using CCEnvs.Extensions;

namespace CCEnvs
{
    public static class TypeExtensions
    {
        public static bool TrySwitchType(this Type source,
            params (Type onType, Action<Type> action)[] conditions)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            var convertedConditions = conditions.Select<(Type onType, Action<Type> action), (Predicate<Type?>, Action<Type>)>(condition => ((inputType) => inputType is not null && inputType.IsType(condition.onType), condition.action)).ToArray();
            return ObjectExtensions.TrySwitch(source, convertedConditions);
        }
        public static bool TrySwitchType<TResult>(this Type source,
            out TResult result,
            params (Type onType, Func<Type, TResult> func)[] conditions)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            var convertedConditions = conditions.Select<(Type onType, Func<Type, TResult> func), (Predicate<Type?>, Func<Type, TResult>)>(condition => ((inputType) => inputType is not null && inputType.IsType(condition.onType), condition.func)).ToArray();
            return ObjectExtensions.TrySwitch(source, out result, convertedConditions);
        }
    }
}
