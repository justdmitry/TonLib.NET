namespace TonLibDotNet.Types.Internal
{
    [TLSchema("internal.transactionId lt:int64 hash:bytes = internal.TransactionId")]
    public class TransactionId : TypeBase
    {
        public static readonly TransactionId Empty = new () { Lt = 0, Hash = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=" };

        public long Lt { get; set; }

        public string Hash { get; set; } = string.Empty;
    }
}
