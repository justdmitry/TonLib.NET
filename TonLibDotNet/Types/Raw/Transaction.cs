namespace TonLibDotNet.Types.Raw
{
    [TLSchema("raw.transaction utime:int53 data:bytes transaction_id:internal.transactionId fee:int64 storage_fee:int64 other_fee:int64 in_msg:raw.message out_msgs:vector<raw.message> = raw.Transaction")]
    public class Transaction : TypeBase
    {
        public Transaction(Internal.TransactionId transactionId)
        {
            TransactionId = transactionId ?? throw new ArgumentNullException(nameof(transactionId));
        }

        public DateTimeOffset Utime { get; set; }

        public string Data { get; set; } = string.Empty;

        public Internal.TransactionId TransactionId { get; set; }

        public long Fee { get; set; }

        public long StorageFee { get; set; }

        public long OtherFee { get; set; }

        public Message? InMsg { get; set; }

        public List<Message>? OutMsgs { get; set; }
    }
}
