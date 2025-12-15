using System.Text.Json.Serialization;

namespace CCEnvs
{
    public interface IPolymorhicSerialiazable
    {
        [JsonInclude]
        [JsonPropertyName("$type")]
        string SelfTypeReference { get; }
    }
}
