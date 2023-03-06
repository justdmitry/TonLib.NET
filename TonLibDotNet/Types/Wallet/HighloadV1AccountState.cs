namespace TonLibDotNet.Types.Wallet
{
    [TLSchema("wallet.highload.v1.accountState wallet_id:int64 seqno:int32 = AccountState")]
    public class HighloadV1AccountState : AccountState
    {
        public long WalletId { get; set; }

        public int Seqno { get; set; }
    }
}
