using System.Text.Json.Serialization;

namespace TonLibDotNet.Types
{
    /// <remarks>
    /// TL Schema:
    /// <code>config config:string blockchain_name:string use_callbacks_for_network:Bool ignore_cache:Bool = Config;</code>
    /// </remarks>
    public class Config : TypeBase
    {
        public Config()
        {
            TypeName = "config";
        }

        [JsonPropertyName("config")]
        public string ConfigJson { get; set; } = string.Empty;

        public string BlockchainName { get; set; } = string.Empty;

        public bool UseCallbacksForNetwork { get; set; } = false;

        public bool IgnoreCache { get; set; } = false;
    }
}
