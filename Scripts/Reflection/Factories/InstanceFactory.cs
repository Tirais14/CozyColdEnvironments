using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UTIRLib.Reflection.Diagnostics;

#nullable enable
namespace UTIRLib.Reflection
{
    public static class InstanceFactory
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static object Create(Type type,
                                    TypeMemberParameters parameters,
                                    params KeyValuePair<Type, object>[] args)
        {
            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Type[] ctorSignature = args.Select(x => x.Key).ToArray();
            ConstructorInfo? ctor = type.GetConstructor(parameters.BindingFlags,
                                                        parameters.Binder,
                                                        parameters.CallingConvention,
                                                        ctorSignature,
                                                        parameters.ParameterModifiers)
                ??
                throw new ConstructorNotFoundException(parameters);

            object[] ctorArgs = args.Select(x => x.Value).ToArray();
            return ctor.Invoke(ctorArgs);
        }
        public static T Create<T>(TypeMemberParameters parameters,
                                  params KeyValuePair<Type, object>[] args)
        {
            return (T)Create(typeof(T), parameters, args);
        }
    }
}
