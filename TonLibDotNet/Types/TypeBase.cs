using System.Text.Json.Serialization;

namespace TonLibDotNet.Types
{
    public abstract class TypeBase
    {
        [JsonIgnore]
        [Obsolete]
        public string TypeName { get; protected set; } = string.Empty;
    }
}
