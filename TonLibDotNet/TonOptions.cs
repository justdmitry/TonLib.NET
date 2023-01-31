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
        /// TonLib timeout when making calls to LiteServer.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(15);

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
    }
}
