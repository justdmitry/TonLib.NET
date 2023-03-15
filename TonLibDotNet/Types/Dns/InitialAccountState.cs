namespace TonLibDotNet.Types.Dns
{
    [TLSchema("dns.initialAccountState public_key:string wallet_id:int64 = InitialAccountState")]
    public class InitialAccountState : Types.InitialAccountState
    {
        public string PublicKey { get; set; } = string.Empty;

        public long WalletId { get; set; }
    }
}
