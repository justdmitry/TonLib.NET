namespace TonLibDotNet.Types.Internal
{
    [TLSchema("internal.transactionId lt:int64 hash:bytes = internal.TransactionId")]
    public class TransactionId : TypeBase, IEquatable<TransactionId>
    {
        private const long EmptyLt = 0L;
        private const string EmptyHash = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";

        public static readonly TransactionId Empty = new () { Lt = EmptyLt, Hash = EmptyHash };

        public long Lt { get; set; }

        public string Hash { get; set; } = string.Empty;

        public bool IsEmpty()
        {
            return this.Equals(Empty);
        }

        #region Equality overloading

        public static bool operator ==(TransactionId? a, TransactionId? b)
        {
            return a is null ? b is null : a.Equals(b);
        }

        public static bool operator !=(TransactionId? a, TransactionId? b)
        {
            return !(a == b);
        }

        public virtual bool Equals(TransactionId? other)
        {
            return other is not null
                && other.Lt == Lt
                && StringComparer.Ordinal.Equals(other.Hash, Hash);
        }

        public override bool Equals(object? obj)
        {
            return obj is TransactionId other
                && Equals(other);
        }

        public override int GetHashCode()
        {
            return Lt.GetHashCode() ^ Hash.GetHashCode();
        }

        #endregion
    }
}
