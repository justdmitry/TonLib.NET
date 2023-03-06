namespace TonLibDotNet.Types.Wallet
{
    [TLSchema("wallet.highload.v1.initialAccountState public_key:string wallet_id:int64 = InitialAccountState")]
    public class HighloadV1InitialAccountState : InitialAccountState
    {
        public string PublicKey { get; set; } = string.Empty;

        public long WalletId { get; set; }
    }
}
