namespace TonLibDotNet.Types.Raw
{
    [TLSchema("raw.fullAccountState balance:int64 code:bytes data:bytes last_transaction_id:internal.transactionId block_id:ton.blockIdExt frozen_hash:bytes sync_utime:int53 = raw.FullAccountState")]
    public class FullAccountState : TypeBase
    {
        public FullAccountState(Internal.TransactionId lastTransactionId, Ton.BlockIdEx blockId)
        {
            LastTransactionId = lastTransactionId ?? throw new ArgumentNullException(nameof(lastTransactionId));
            BlockId = blockId ?? throw new ArgumentNullException(nameof(blockId));
        }

        public long Balance { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Data { get; set; } = string.Empty;

        public Internal.TransactionId LastTransactionId { get; set; }

        public Ton.BlockIdEx BlockId { get; set; }

        public string FrozenHash { get; set; } = string.Empty;

        public DateTimeOffset SyncUtime { get; set; }
    }
}
