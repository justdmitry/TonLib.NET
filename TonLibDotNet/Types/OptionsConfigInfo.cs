namespace TonLibDotNet.Types
{
    /// <remarks>
    /// TL Schema:
    /// <code>options.configInfo default_wallet_id:int64 default_rwallet_init_public_key:string = options.ConfigInfo;</code>
    /// </remarks>
    public class OptionsConfigInfo : TypeBase
    {
        public OptionsConfigInfo()
        {
            TypeName = "options.configInfo";
        }

        public string DefaultWalletId { get; set; } = string.Empty;

        public string DefaultRwalletInitPublicKey { get; set; } = string.Empty;
    }
}
