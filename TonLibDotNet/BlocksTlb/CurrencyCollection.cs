using TonLibDotNet.Cells;

namespace TonLibDotNet.BlocksTlb
{
    /// <remarks><code>
    /// currencies$_ grams:Grams other:ExtraCurrencyCollection = CurrencyCollection
    /// </code></remarks>
    public class CurrencyCollection
    {
        public CurrencyCollection(Slice src)
        {
            Grams = src.LoadCoins();
            Other = new ExtraCurrencyCollection(src);
        }

        public long Grams { get; set; }

        public ExtraCurrencyCollection Other { get; set; }
    }
}
