using CCEnvs.Attributes;
using CCEnvs.Json;
using CCEnvs.Json.Converters;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.Json.Converters;

#nullable enable
namespace CCEnvs.Unity.Json
{
    [CCConfigurationInstaller]
    internal static class JsonConfigInstaller
    {
        public static void Install()
        {
            JsonSettingsProvider.ReplaceOrAddConverters(
                new Vector2Converter(),
                new Vector2IntConverter(),
                new Vector3Converter(),
                new Vector3IntConverter(),
                new TypedDtoJsonConverter<IStorageItemDto, IStorageItem>(),
                new TypedDtoJsonConverter<IItemStackDto, IItemStack>(),
                new CommonDtoJsonConverter<ItemStackDto, ItemStack>()
                );
        }
    }
}
