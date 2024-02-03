using System.Text.Json.Nodes;
using TonLibDotNet.Utils;

namespace TonLibDotNet
{
    public class TonOptions
    {
        /// <summary>
        /// Path to JSON config for testnet (default value is from https://ton.org/docs/develop/dapps/apis/adnl).
        /// </summary>
        /// <remarks>
        /// Unless <see cref="ConfigPathLocalTestnet"/> is set, this url will be used to obtain config file on every TonClient init.
        /// </remarks>
        public Uri ConfigPathTestnet { get; set; } = new ("https://ton.org/testnet-global.config.json");

        /// <summary>
        /// Path to JSON config for mainnet (default value is from https://ton.org/docs/develop/dapps/apis/adnl).
        /// </summary>
        /// <remarks>
        /// Unless <see cref="ConfigPathLocalMainnet"/> is set, this url will be used to obtain config file on every TonClient init.
        /// </remarks>
        public Uri ConfigPathMainnet { get; set; } = new("https://ton.org/global-config.json");

        /// <summary>
        /// Path to local file with JSON config for testnet (default value is empty string).
        /// </summary>
        /// <remarks>
        /// Download config file and set this property to prevent downloading config from internet (see <see cref="ConfigPathLocalTestnet"/>) every time.
        /// </remarks>
        public string ConfigPathLocalTestnet { get; set; } = string.Empty;

        /// <summary>
        /// Path to local file with JSON config for mainnet (default value is empty string).
        /// </summary>
        /// <remarks>
        /// Download config file and set this property to prevent downloading config from internet (see <see cref="ConfigPathLocalMainnet"/>) every time.
        /// </remarks>
        public string ConfigPathLocalMainnet { get; set; } = string.Empty;

        /// <summary>
        /// 'True' by default. Set to 'false' to use testnet config.
        /// </summary>
        public bool UseMainnet { get; set; } = true;

        /// <summary>
        /// Max amount of time TonClient will wait for access to tonlib when used as singleton in multithreaded scenarios.
        /// </summary>
        /// <remarks>See also <see cref="TonClientTimeout"/> and <see cref="TonClientSyncTimeout"/>.</remarks>
        /// <seealso cref="TonClientTimeout"/>
        /// <seealso cref="TonClientSyncTimeout"/>
        public TimeSpan ConcurrencyTimeout { get; set; } = TimeSpan.FromSeconds(21); // A bit more than TonClientTimeout

        /// <summary>
        /// Max amount of time TonClient will wait for valid (synced!) response from tonlib (except <see cref="Requests.Sync"/> request).
        /// </summary>
        /// <remarks>See also <see cref="TonClientSyncTimeout"/> and <see cref="ConcurrencyTimeout"/>.</remarks>
        /// <seealso cref="TonClientSyncTimeout"/>
        /// <seealso cref="ConcurrencyTimeout"/>
        public TimeSpan TonClientTimeout { get; set; } = TimeSpan.FromSeconds(20);

        /// <summary>
        /// Max amount of time TonClient will wait for valid (synced!) response from tonlib for <see cref="Requests.Sync"/> request.
        /// </summary>
        /// <remarks>See also <see cref="TonClientTimeout"/> and <see cref="ConcurrencyTimeout"/>.</remarks>
        /// <seealso cref="TonClientTimeout"/>
        /// <seealso cref="ConcurrencyTimeout"/>
        public TimeSpan TonClientSyncTimeout { get; set; } = TimeSpan.FromSeconds(60);

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
