using TonLibDotNet.Cells;

namespace TonLibDotNet.BlocksTlb
{
    public abstract partial class TransactionDescr
    {
        /// <summary>
        /// Transaction description (ordinary tx).
        /// </summary>
        /// <remarks>
        /// <code>
        /// trans_ord$0000 credit_first:Bool
        ///   storage_ph:(Maybe TrStoragePhase)
        ///   credit_ph:(Maybe TrCreditPhase)
        ///   compute_ph:TrComputePhase action:(Maybe ^TrActionPhase)
        ///   aborted:Bool bounce:(Maybe TrBouncePhase)
        ///   destroyed:Bool
        ///   = TransactionDescr;
        /// </code></remarks>
        public class Ord : TransactionDescr
        {
            public Ord(Slice src)
            {
                CreditFirst = src.LoadBit();

                if (src.LoadBit())
                {
                    StoragePh = new TrStoragePhase(src);
                }

                if (src.LoadBit())
                {
                    CreditPh = new TrCreditPhase(src);
                }

                ComputePh = TrComputePhase.CreateFrom(src);

                if (src.LoadBit())
                {
                    Action = src.LoadRef();
                }

                Aborted = src.LoadBit();
            }

            public override bool IsOrd => true;

            public bool CreditFirst { get; set; }

            public TrStoragePhase? StoragePh { get; set; }

            public TrCreditPhase? CreditPh { get; set; }

            public TrComputePhase ComputePh { get; set; }

            public Cell? Action { get; set; }

            public bool Aborted { get; set; }
        }
    }
}
