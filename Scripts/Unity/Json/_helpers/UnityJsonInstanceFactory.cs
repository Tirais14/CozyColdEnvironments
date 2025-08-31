using CCEnvs.Json.DTO;
using CCEnvs.Reflection;
using CCEnvs.Reflection.ObjectModel;
using System;

#nullable enable
namespace CCEnvs.Unity.Json
{
    public static class UnityJsonInstanceFactory
    {
        public static T Create<TDto, T>(TDto dto)
            where TDto : IJsonDto
        {
            if (UnityJsonSettingsProvider.TryGetFactoryByDtoMethod(out Func<TDto, T>? method))
                return InstanceFactory.CreateBy<T>(dto);

            return InstanceFactory.Create<T>(
                new ConstructorBindings
                {
                    BindingFlags = BindingFlagsDefault.InstanceAll,
                    Arguments = new ExplicitArguments(dto)
                }, 
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound);
        }
    }
}
