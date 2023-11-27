namespace TonLibDotNet.Types.Wallet
{
    [TLSchema("wallet.v4.initialAccountState public_key:string wallet_id:int64 = InitialAccountState")]
    public class V4InitialAccountState : InitialAccountState
    {
        public string PublicKey { get; set; } = string.Empty;

        public long WalletId { get; set; }
    }
}
