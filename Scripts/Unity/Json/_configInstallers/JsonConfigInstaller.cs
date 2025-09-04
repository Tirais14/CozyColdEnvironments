using CCEnvs.Attributes;
using CCEnvs.Collections;
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
            JsonSettingsProvider.Converters.AddRange(
                new Vector2Converter(),
                new Vector2IntConverter(),
                new Vector3Converter(),
                new Vector3IntConverter(),
                new TypedDtoJsonConverter<IItemContainerDto, IItemStack>(),
                new CommonDtoJsonConverter<IItemContainerDto<ItemStack>, ItemStack>()
                );
        }
    }
}
