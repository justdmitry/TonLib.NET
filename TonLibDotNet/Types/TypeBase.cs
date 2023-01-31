using System.Text.Json.Serialization;

namespace TonLibDotNet.Types
{
    public abstract class TypeBase
    {
        [JsonPropertyName("@type")]
        public string TypeName { get; protected set; } = string.Empty;
    }
}
