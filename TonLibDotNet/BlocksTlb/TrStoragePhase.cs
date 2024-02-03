using TonLibDotNet.Cells;

namespace TonLibDotNet.BlocksTlb
{
    /// <remarks>
    /// <code>
    /// tr_phase_storage$_ storage_fees_collected:Grams
    ///     storage_fees_due:(Maybe Grams)
    ///     status_change:AccStatusChange
    ///     = TrStoragePhase;
    /// </code></remarks>
    public class TrStoragePhase
    {
        public TrStoragePhase(Slice src)
        {
            StorageFeesCollected = src.LoadCoins();

            if (src.LoadBit())
            {
                StorageFeesDue = src.LoadCoins();
            }

            StatusChange = AccStatusChange.CreateFrom(src);
        }

        public long StorageFeesCollected { get; set; }

        public long StorageFeesDue { get; set; }

        public AccStatusChange StatusChange { get; set; }
    }
}
