using TonLibDotNet.Cells;

namespace TonLibDotNet.BlocksTlb
{
    /// <remarks><code>
    /// tr_phase_credit$_ due_fees_collected:(Maybe Grams) credit:CurrencyCollection = TrCreditPhase;
    /// </code></remarks>
    public class TrCreditPhase
    {
        public TrCreditPhase(Slice src)
        {
            if (src.LoadBit())
            {
                DueFeesCollected = src.LoadCoins();
            }

            Credit = new CurrencyCollection(src);
        }

        public long DueFeesCollected { get; set; }

        public CurrencyCollection Credit { get; set; }
    }
}
