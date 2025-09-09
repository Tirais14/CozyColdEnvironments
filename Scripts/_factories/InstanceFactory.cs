using CCEnvs.Attributes;
using CCEnvs.Collections;
using CCEnvs.Common;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Cached;
using CCEnvs.Reflection.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using static CCEnvs.BindingFlagsDefault;

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
            NonPublic = 4,
            Default = ThrowIfNotFound,
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static object Create(Type type,
                                    ExplicitArguments arguments = default,
                                    Parameters parameters = Parameters.Default)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsInterface)
                throw new ArgumentException($"Type {type.GetName()} is interface and not allowed to create.");
            if (type.IsAbstract)
                throw new ArgumentException($"Type {type.GetName()} is abstract and not allowed to create.");
            if (arguments.IsDefault())
                arguments = ExplicitArguments.Empty;

            bool nonPublic = parameters.IsFlagSetted(Parameters.NonPublic);
            BindingFlags bindingFlags = ResolveBindingFlags(nonPublic);
            if (!GetConstructorByCache(out MethodBase? ctor))
            {
                ctor = GetConstructor();

                if (ctor is null)
                {
                    if (parameters.HasFlag(Parameters.ThrowIfNotFound))
                        ThrowNotFound();

                    return null!;
                }

                if (parameters.HasFlag(Parameters.CacheConstructor))
                    TypeCache.TryCacheMember(ctor);
            }

            object?[] ctorArgs = (object?[])arguments;
            if (ctor is ConstructorInfo typedCtor)
                return typedCtor.Invoke(ctorArgs);

            return ctor.Invoke(null, ctorArgs);

            bool GetConstructorByCache([NotNullWhen(true)] out MethodBase? result)
            {
                TypeCache.TryGetConstructor(
                    new TypeCache.MethodKey(
                        type,
                        (CCParameters)arguments,
                        arguments.GetParameterModifiers()),
                    out ConstructorInfo? ctor);

                result = ctor;

                if (result is null)
                {
                    TypeCache.TryGetMethod(
                        new TypeCache.MethodKey(
                            type,
                            (CCParameters)arguments,
                            arguments.GetParameterModifiers()),
                        out MethodInfo? method);

                    result = method;
                }

                return result is not null;
            }

            void ThrowNotFound()
            {
                throw new ConstructorNotFoundException(
                    type,
                    bindingFlags,
                    (CCParameters)arguments);
            }

            MethodBase? GetConstructor()
            {
                bindingFlags |= BindingFlags.Instance;
                MethodBase? ctor = type.GetConstructor(bindingFlags,
                    binder: null,
                    arguments.GetTypes(),
                    CC.Create.Array(arguments.GetParameterModifiers()));

                if (ctor is null)
                {
                    bindingFlags = bindingFlags.ResetFlag(BindingFlags.Instance)
                                               .SetFlag(BindingFlags.Static);

                    MethodInfo[] methods =
                        (from x in type.ForceGetMethods(bindingFlags)
                         where x.IsDefined<InstanceConstructorAttribute>(inherit: true)
                         ||
                         x.Name == "Create"
                         where x.ReturnType.IsType(type)
                         select x)
                         .ToArray();

                    ctor = methods.FirstOrDefault(
                        x => x.GetCCParameters() == (CCParameters)arguments);
                }

                return ctor;
            }
        }

        public static T Create<T>(ExplicitArguments arguments = default,
                                  Parameters parameters = Parameters.Default,
                                  Type? type = null)
        {
            type ??= typeof(T);

            return (T)Create(type, arguments, parameters);
        }

        /// <summary>
        /// Creates type by fields and props in data and by comparing it names.
        /// Skips readonly fields and properties without setter.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static object _CreateBy(Type type,
                                      object data,
                                      Parameters parameters = Parameters.Default)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (data.IsNull())
                throw new ArgumentNullException(nameof(data));

            object created;
            CreateByEmptyConstructor();

            if (created.IsNull())
                CreateByAnyConstructor();

            if (created.IsNull())
            {
                if (parameters.IsFlagSetted(Parameters.ThrowIfNotFound))
                    throw new ConstructorNotFoundException(
                        type,
                        InstanceAll,
                        CCParameters.Empty);

                return null!;
            }

            Type dataType = data.GetType();

            InjectProperties();
            InjectFields();

            return created;
            void CreateByEmptyConstructor()
            {
                created = Create(type,
                    ExplicitArguments.EmptyIgnoreOptional,
                    parameters.ResetFlag(Parameters.ThrowIfNotFound));
            }

            void CreateByAnyConstructor()
            {
                (ConstructorInfo ctor, ParameterInfo[] parameters) pair =
                    (from x in type.GetConstructors(BindingFlagsDefault.InstanceAll)
                     select (ctor: x, parameters: x.GetParameters()) into p
                     orderby p.parameters.Length
                     select p).FirstOrDefault();

                if (pair.ctor is null)
                    return;

                object[] args = new object[pair.parameters.Length];

                created = pair.ctor.Invoke(args);
            }

            void InjectProperties()
            {
                PropertyInfo[] props = dataType.ForceGetProperties(
                    BindingFlagsDefault.InstanceAll)
                    .Where(x => x.CanWrite && x.CanRead)
                    .ToArray();

                PropertyInfo[] createdProps = type.ForceGetProperties(
                    BindingFlagsDefault.InstanceAll)
                    .Where(x => x.CanWrite && x.CanRead)
                    .ToArray();

                foreach (var createdProp in createdProps)
                {
                    if (props.Find(x => x.Name.Equals(createdProp.Name))
                        is PropertyInfo foundProp
                        )
                        createdProp.SetValue(created, foundProp.GetValue(data));
                }
            }

            void InjectFields()
            {
                FieldInfo[] fields = dataType.ForceGetFields(
                    BindingFlagsDefault.InstanceAll)
                    .Where(x => !x.IsInitOnly)
                    .ToArray();

                FieldInfo[] createdFields = type.ForceGetFields(
                    BindingFlagsDefault.InstanceAll)
                    .Where(x => !x.IsInitOnly)
                    .ToArray();

                foreach (var createdField in createdFields)
                {
                    if (fields.Find(x => x.Name.Equals(createdField.Name))
                        is FieldInfo foundField
                        )
                        createdField.SetValue(created, foundField.GetValue(data));
                }
            }
        }

        /// <summary>
        /// <see cref="_CreateBy(Type, object, Parameters)"/>
        /// </summary>
        public static T _CreateBy<T>(object data, Parameters parameters = Parameters.Default)
        {
            return (T)_CreateBy(typeof(T), data, parameters);
        }

        private static BindingFlags ResolveBindingFlags(bool nonPublic)
        {
            if (nonPublic)
                return BindingFlags.Public | BindingFlags.NonPublic;

            return BindingFlags.Public;
        }
    }
}
