namespace TonLibDotNet.Types.Internal
{
    [TLSchema("internal.transactionId lt:int64 hash:bytes = internal.TransactionId")]
    public class TransactionId : TypeBase
    {
        public long Lt { get; set; }

        public string Hash { get; set; } = string.Empty;
    }
}
