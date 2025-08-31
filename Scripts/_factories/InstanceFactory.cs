using System;
using System.Reflection;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Cached;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection.ObjectModel;
using System.Linq;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs
{
    public static class InstanceFactory
    {
        [Flags]
        public enum Parameters
        {
            None,
            CacheConstructor,
            ThrowIfNotFound = 2,
            Default = ThrowIfNotFound,
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static object Create(Type type,
                                    ConstructorBindings bindings,
                                    Parameters parameters = Parameters.Default)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsInterface)
                throw new ArgumentException($"Type {type.GetName()} is interface and not allowed to create.");
            if (bindings is null)
                throw new ArgumentNullException(nameof(bindings));

            bool throwIfNotFound = parameters.HasFlag(Parameters.ThrowIfNotFound);
            if (!TypeCache.TryGetConstructor(
                new TypeCache.ConstructrorKey(
                    type,
                    (Type[])bindings.Arguments,
                    bindings.ParameterModifiers),
                out ConstructorInfo? ctor))
            {
                ctor = type.GetConstructor(bindings, throwIfNotFound: false);

                if (ctor is null)
                {
                    if (throwIfNotFound)
                        throw new ConstructorNotFoundException(
                            type,
                            bindings.BindingFlags,
                            bindings.Arguments.signature);

                    return null!;
                }

                if (parameters.HasFlag(Parameters.CacheConstructor))
                    TypeCache.TryCacheMember(ctor);
            }

            object?[] ctorArgs = (object?[])bindings.Arguments;
            return ctor.Invoke(ctorArgs);
        }
        public static object Create(Type type,
                                    ExplicitArguments args,
                                    Parameters parameters = Parameters.Default)
        {
            return Create(type, new ConstructorBindings
            {
                BindingFlags = BindingFlagsDefault.InstanceAll,
                Arguments = args
            }, parameters);
        }

        public static T Create<T>(ConstructorBindings constructorParams,
            Parameters parameters = Parameters.Default)
        {
            return (T)Create(typeof(T), constructorParams, parameters);
        }
        public static T Create<T>(Type type,
                                  ExplicitArguments args,
                                  Parameters parameters = Parameters.Default)
        {
            return (T)Create(type, args, parameters);
        }

        /// <summary>
        /// Creates type by fields and props in data and by comparing it names.
        /// Skips readonly fields and properties without setter.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static object CreateBy(Type type,
                                      object data,
                                      Parameters parameters = Parameters.Default)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (data.IsNull())
                throw new ArgumentNullException(nameof(data));

            object created = Create(type,
                new ConstructorBindings
                {
                    BindingFlags = BindingFlagsDefault.InstanceAll,
                    Arguments = ExplicitArguments.Empty
                },
                parameters);

            Type dataType = data.GetType();

            FieldInfo[] fields = dataType.ForceGetFields(
                BindingFlagsDefault.InstanceAll)
                .Where(x => !x.IsInitOnly)
                .ToArray();

            IEnumerable<FieldInfo> createdFields = type.ForceGetFields(
                BindingFlagsDefault.InstanceAll)
                .Where(x => !x.IsInitOnly);

            foreach (var createdField in createdFields)
            {
                if (fields.Find(x => x.Name.Equals(createdField.Name))
                    is FieldInfo foundField
                    )
                    createdField.SetValue(created, foundField.GetValue(data));
            }

            PropertyInfo[] props = dataType.ForceGetProperties(
                BindingFlagsDefault.InstanceAll)
                .Where(x => x.CanWrite && x.CanRead)
                .ToArray();

            IEnumerable<PropertyInfo> createdProps = type.ForceGetProperties(
                BindingFlagsDefault.InstanceAll)
                .Where(x => x.CanWrite && x.CanRead);

            foreach (var createdProp in createdProps)
            {
                if (props.Find(x => x.Name.Equals(createdProp.Name))
                    is PropertyInfo foundProp
                    )
                    createdProp.SetValue(created, foundProp.GetValue(data));
            }

            return created;
        }

        /// <summary>
        /// <see cref="CreateBy(Type, object, Parameters)"/>
        /// </summary>
        public static T CreateBy<T>(object data, Parameters parameters = Parameters.Default)
        {
            return (T)CreateBy(typeof(T), data, parameters);
        }
    }
}
