using CCEnvs.Attributes;
using CCEnvs.Collections;
using CCEnvs.Json;
using CCEnvs.Json.Converters;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.Json.Converters;

#nullable enable
#pragma warning disable S1144
#pragma warning disable IDE0051
namespace CCEnvs.Unity.Json
{
    [CCStaticInstaller]
    public static class JsonStaticInstaller
    {
        [CCStaticInstallerMethod]
        private static void Main()
        {
            JsonSettingsProvider.Converters.AddRange(
                new Vector2Converter(),
                new Vector2IntConverter(),
                new Vector3Converter(),
                new Vector3IntConverter(),
                new TypedDtoJsonConverter<IItemContainerDto, IItemContainer>()
                );
        }
    }
}
