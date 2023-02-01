namespace TonLibDotNet.Types.Wallet
{
    [TLSchema("wallet.v3.accountState wallet_id:int64 seqno:int32 = AccountState")]
    public class V3AccountState : AccountState
    {
        public long WalletId { get; set; }

        public int Seqno { get; set; }
    }
}
