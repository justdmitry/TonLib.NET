using System.Text.Json.Serialization;

namespace TonLibDotNet.Requests
{
    public abstract class RequestBase : Types.TypeBase
    {
        [JsonIgnore]
        public bool IsStatic { get; protected set; } = false;
    }
}
