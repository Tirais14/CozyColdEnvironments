#nullable enable
using CCEnvs.Attributes;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;
using System.Diagnostics;
using System.Linq;

namespace CCEnvs.Conversations
{
    public static class TypeMutator
    {
        /// <summary>
        /// Simillar to <see cref="Convert"/>, but supports more variants to convert object.
        /// <br/>Supported and do in next order:
        /// <br/>-<see cref="Convert.ChangeType(object, Type)"/>
        /// <br/>-<see cref="IMutable"/>
        /// <br/>-overloaded operators
        /// <br/>-static methods marked with <see cref="ConverterAttribute"/>
        /// <br/>-<see cref="InstanceFactory.Create(Type, ExplicitArguments, InstanceFactory.Parameters)"/>. Must contain constructor which take input object.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static object MutateType(object input, Type toType)
        {
            CC.Guard.IsNotNull(input, nameof(input));
            CC.Guard.IsNotNull(toType, nameof(toType));

            Type inputType = input.GetType();
            if (inputType.IsType(toType))
                return input;

            var converted = ConvertByDefaultConverter(input, toType).Resolve()
                .Else(() => ConvertByInterface(input).Access()!)
                .Else(() => ConvertByOverloadedCastOperator(input, inputType, toType).Access()!)
                .Else(() => ConvertByCustomConverter(input, inputType, toType).Access()!)
                .Else(() => CreateByReflection(input, toType).Access()!)
                .Else(() => throw new InvalidOperationException($"Cannot mutate type: {inputType.GetFullName()} to type: {toType.GetFullName()}."))
                .AccessUnsafe();

            return converted;
        }
        /// <inheritdoc cref="MutateType(object, Type)"/>
        [DebuggerStepThrough]
        public static T MutateType<T>(object target)
        {
            return (T)MutateType(target, typeof(T));
        }

        private static Maybe<object> ConvertByDefaultConverter(object input, Type toType)
        {
            return new Catched<object>(() => Convert.ChangeType(input, toType)).Access();
        }

        private static Maybe<object> ConvertByOverloadedCastOperator(object input, Type type, Type toType)
        {
            return type.Catch()
                       .Map(t => t.GetOverloadedCastOperator(toType).Raw)
                       .Map(x => x.Invoke(input, Range.From(input)))
                       .Access();
        }

        private static Maybe<object> ConvertByInterface(object input)
        {
            return input.AsOrDefault<IMutable>()
                        .Map(x => x.MutateType())
                        .Access();
        }

        private static Maybe<object> ConvertByCustomConverter(object input, Type type, Type toType)
        {
            return type.ReflectQuery()
                .NonPublic()
                .Attributes(typeof(ConverterAttribute))
                .ArgumentTypes(type)
                .ExtraType(toType)
                .Arguments(input)
                .Invoke();
        }

        private static Maybe<object> CreateByReflection(object arg, Type type)
        {
            return type.ReflectQuery()
                       .Instance()
                       .Arguments(arg)
                       .Invoke();
        }
    }
}
