namespace TonLibDotNet.Types.Wallet
{
    [TLSchema("wallet.highload.v2.initialAccountState public_key:string wallet_id:int64 = InitialAccountState")]
    public class HighloadV2InitialAccountState : InitialAccountState
    {
        public string PublicKey { get; set; } = string.Empty;

        public long WalletId { get; set; }
    }
}
