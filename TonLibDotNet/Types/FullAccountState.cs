namespace TonLibDotNet.Types
{
    [TLSchema("fullAccountState address:accountAddress balance:int64 last_transaction_id:internal.transactionId block_id:ton.blockIdExt sync_utime:int53 account_state:AccountState revision:int32 = FullAccountState")]
    public class FullAccountState : TypeBase
    {
        public FullAccountState(AccountAddress address, Internal.TransactionId lastTransactionId, Ton.BlockIdEx blockId, AccountState accountState)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            LastTransactionId = lastTransactionId ?? throw new ArgumentNullException(nameof(lastTransactionId));
            BlockId = blockId ?? throw new ArgumentNullException(nameof(blockId));
            AccountState = accountState ?? throw new ArgumentNullException(nameof(accountState));
        }

        public AccountAddress Address { get; set; }

        public long Balance { get; set; }

        public Internal.TransactionId LastTransactionId { get; set; }

        public Ton.BlockIdEx BlockId { get; set; }

        public DateTimeOffset SyncUtime { get; set; }

        public AccountState AccountState { get; set; }

        public int Revision { get; set; }
    }
}
