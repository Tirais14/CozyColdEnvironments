using CCEnvs.Diagnostics;
using CCEnvs.Json;
using CCEnvs.Json.DTO;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

#nullable enable
namespace CCEnvs.Unity.Json
{
    public static class UnityJsonSettingsProvider
    {
        public static JsonConverter[] Converters { get; private set; } = new JsonConverter[]
        {
            new Vector2Converter(),
            new Vector2IntConverter(),
            new Vector3Converter(),
            new Vector3IntConverter(),
            new TypedJsonConverter<StorageItemDto, IStorageItem>(),
            new TypedJsonConverter<ItemStackDto, IItemStack>()
        };
        private readonly static Dictionary<Type, Delegate> factoryByDtoMethods = new();

        /// <exception cref="CollectionArgumentException"></exception>
        public static void AddConverters(params JsonConverter[] converters)
        {
            if (converters.IsNullOrEmpty())
                throw new CollectionArgumentException(nameof(converters), converters);

            Converters = Converters.Concat(converters).ToArray();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static void ReplaceOrAddConverter(JsonConverter converter)
        {
            if (converter is null)
                throw new ArgumentNullException(nameof(converter));

            Type converterType = converter.GetType();
            for (int i = 0; i < Converters.Length; i++)
            {
                if (Converters[i].GetType() == converterType)
                    Converters[i] = converter;
            }

            AddConverters(converter);
        }


        /// <exception cref="ArgumentNullException"></exception>
        public static void ReplaceOrAddConverters(params JsonConverter[] converters)
        {
            if (converters is null)
                throw new ArgumentNullException(nameof(converters));

            for (int i = 0; i < converters.Length; i++)
                ReplaceOrAddConverter(converters[i]);
        }

        public static void AddOrReplaceFactoryByDtoMethod<TDto, T>(Func<TDto, T> func)
            where TDto : IJsonDTO
        {
            if (factoryByDtoMethods.ContainsKey(typeof(T)))
            {
                factoryByDtoMethods[typeof(TDto)] = func;
                return;
            }

            factoryByDtoMethods.Add(typeof(TDto), func);
        }

        public static bool TryGetFactoryByDtoMethod<TDto, T>(
            [NotNullWhen(true)] out Func<TDto, T>? result)
            where TDto : IJsonDTO
        {
            if (factoryByDtoMethods.TryGetValue(typeof(T), out Delegate methodUntyped))
            {
                result = (Func<TDto, T>)methodUntyped;
                return true;
            }

            result = null;
            return false;
        }

        public static JsonSerializerSettings GetSettings(object? context = null,
            StreamingContextStates contextStates = StreamingContextStates.File)
        {
            JsonSerializerSettings defaultSettings = JsonConvert.DefaultSettings?.Invoke()
                                                     ??
                                                     new JsonSerializerSettings();

            defaultSettings.Context = new StreamingContext(contextStates, context);
            defaultSettings.Converters = defaultSettings.Converters.Concat(Converters).ToArray();

            return defaultSettings;
        }
    }
}
