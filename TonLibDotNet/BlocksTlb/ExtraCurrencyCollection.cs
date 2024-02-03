using TonLibDotNet.Cells;

namespace TonLibDotNet.BlocksTlb
{
    /// <remarks><code>
    /// extra_currencies$_ dict:(HashmapE 32 (VarUInteger 32)) = ExtraCurrencyCollection;
    /// </code></remarks>
    public class ExtraCurrencyCollection
    {
        public ExtraCurrencyCollection(Slice src)
        {
            Dict = src.TryLoadDict();
        }

        public Cell? Dict { get; set; }
    }
}
