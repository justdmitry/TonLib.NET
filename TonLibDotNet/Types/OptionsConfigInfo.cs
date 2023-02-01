namespace TonLibDotNet.Types
{
    [TLSchema("options.configInfo default_wallet_id:int64 default_rwallet_init_public_key:string = options.ConfigInfo")]
    public class OptionsConfigInfo : TypeBase
    {
        public long DefaultWalletId { get; set; }

        public string DefaultRwalletInitPublicKey { get; set; } = string.Empty;
    }
}
