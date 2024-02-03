using TonLibDotNet.Cells;

namespace TonLibDotNet.BlocksTlb
{
    /// <summary>
    /// Transaction description.
    /// </summary>
    /// <remarks>
    /// See also: <seealso cref="Ord"/>.
    /// </remarks>
    public abstract partial class TransactionDescr
    {
        public virtual bool IsOrd { get; } = false;

        public static TransactionDescr? CreateFrom(Slice src)
        {
            var prefix = src.PreloadUInt(4);

            if (prefix == 0)
            {
                src.SkipBits(4); // prefix
                return new Ord(src);
            }

            return null;
        }
    }
}
