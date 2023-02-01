using System.Text.Json.Nodes;

namespace TonLibDotNet
{
    public class TonOptions
    {
        /// <summary>
        /// Path to JSON config for testnet (default value is from https://ton.org/docs/develop/dapps/apis/adnl).
        /// </summary>
        public Uri ConfigPathTestnet { get; set; } = new ("https://ton.org/testnet-global.config.json");

        /// <summary>
        /// Path to JSON config for mainnet (default value is from https://ton.org/docs/develop/dapps/apis/adnl).
        /// </summary>
        public Uri ConfigPathMainnet { get; set; } = new("https://ton.org/global-config.json");

        /// <summary>
        /// 'True' by default. Set to 'false' to use testnet config.
        /// </summary>
        public bool UseMainnet { get; set; } = true;

        /// <summary>
        /// Max amount of time TonClient will wait for valid (synced!) response from tonlib.
        /// </summary>
        public TimeSpan TonClientTimeout { get; set; } = TimeSpan.FromSeconds(20);

        /// <summary>
        /// TonLib timeout when making calls to tonlib/LiteServer.
        /// </summary>
        public TimeSpan TonLibTimeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// TonLib verbosity level, from 0 (FATAL) to 4 (DEBUG).
        /// </summary>
        public int VerbosityLevel { get; set; } = 1;

        /// <summary>
        /// Request/response text limit for logging (longer are trimmed). Set 0 or negative to disable trimming.
        /// </summary>
        public int LogTextLimit { get; set; } = 200;

        /// <summary>
        /// Options to apply in init() call.
        /// </summary>
        public Types.Options Options { get; set; } = new();

        /// <summary>
        /// Serializer to use.
        /// </summary>
        public ITonJsonSerializer Serializer { get; set; } = new TonJsonSerializer();

        public Func<JsonArray, JsonNode> LiteServerSelector { get; set; } = LiteServerSelectorRandom;

        public static JsonNode LiteServerSelectorFirst(JsonArray liteServers)
        {
            return liteServers[0]!;
        }

        public static JsonNode LiteServerSelectorRandom(JsonArray liteServers)
        {
            var rnd = new Random();
            var index = rnd.Next(liteServers.Count);
            return liteServers[index]!;
        }
    }
}
