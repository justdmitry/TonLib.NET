namespace TonLibDotNet.Types
{
    [TLSchema("fees in_fwd_fee:int53 storage_fee:int53 gas_fee:int53 fwd_fee:int53 = Fees")]
    public class Fees : TypeBase
    {
        public long InFwdFee { get; set; }

        public long StorageFee { get; set; }

        public long GasFee { get; set; }

        public long FwdFee { get; set; }
    }
}
