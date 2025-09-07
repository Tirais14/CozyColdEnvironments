#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;
using System;

namespace CCEnvs
{
    public static class CCConvert
    {
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InstanceCreationException"></exception>
        public static object? Convert(object? target, Type toType)
        {
            if (target.IsNull())
                return null;
            if (toType is null)
                throw new ArgumentNullException(nameof(toType));

            if (target is IConvertibleCC convertible
                &&
                convertible.Convert() is object temp
                &&
                temp.GetType().IsType(toType)
                )
                return temp;

            if (toType.IsInterface || toType.IsAbstract)
                return default;

            object? result = InstanceFactory.Create(toType,
                new ExplicitArguments(new ExplicitArgument(target)),
                InstanceFactory.Parameters.CacheConstructor);

            //if (result.IsNotNull())
            //    return result;

            //result = InstanceFactory.CreateBy(
            //    toType,
            //    dto,
            //    InstanceFactory.Parameters.CacheConstructor);

            if (result.IsNull())
                throw new InstanceCreationException(toType);

            return result;
        }
        public static T? Convert<T>(object? target)
        {
            return (T?)Convert(target, typeof(T));
        }

        /// <exception cref="ArgumentException"></exception>
        public static object? Convert(ITypeProvider? target)
        {
            if (target.IsDefault())
                return default;
            if (target.ObjectType is null)
                throw new ArgumentException(nameof(target.ObjectType));

            return Convert(target, target.ObjectType);
        }
        /// <exception cref="ArgumentException"></exception>
        public static T? Convert<T>(ITypeProvider? target)
        {
            if (target.IsDefault())
                return default;
            if (target.ObjectType is null)
                throw new ArgumentException(nameof(target.ObjectType));
            if (target.ObjectType.IsNotType(typeof(T)))
                throw new ArgumentException(nameof(target.ObjectType));

            return (T?)Convert(target, target.ObjectType);
        }
    }
}
