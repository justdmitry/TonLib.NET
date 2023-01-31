using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    /*
     *
     * Sample request:
     * - too big -
     *
     * Sample response:
     * {"@type":"options.info",
     *  "config_info":{"@type":"options.configInfo",
     *                 "default_wallet_id":"123456789",
     *                 "default_rwallet_init_public_key":"abcdefABCDEF1234567890abcdefABCDEF1234567890abcd"}}
     *
     */
    public class Init : RequestBase<OptionsInfo>
    {
        public Init(Options options)
        {
            ArgumentNullException.ThrowIfNull(options);

            TypeName = "init";
            Options = options;
        }

        public Options Options { get; set; }
    }
}
