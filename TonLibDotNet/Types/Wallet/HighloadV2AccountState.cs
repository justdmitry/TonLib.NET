namespace TonLibDotNet.Types.Wallet
{
    [TLSchema("wallet.highload.v2.accountState wallet_id:int64 = AccountState")]
    public class HighloadV2AccountState : AccountState
    {
        public long WalletId { get; set; }
    }
}
