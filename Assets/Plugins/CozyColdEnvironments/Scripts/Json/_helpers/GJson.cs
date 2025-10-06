#nullable enable
namespace CCEnvs.Json
{
    public static class GJson
    {
        public static string Namespace { get; } = nameof(Newtonsoft) + '.' + nameof(Json);
    }
}
