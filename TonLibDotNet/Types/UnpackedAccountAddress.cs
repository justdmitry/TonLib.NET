namespace TonLibDotNet.Types
{
    [TLSchema("unpackedAccountAddress workchain_id:int32 bounceable:Bool testnet:Bool addr:bytes = UnpackedAccountAddress")]
    public class UnpackedAccountAddress : TypeBase
    {
        public int WorkchainId { get; set; }

        public bool Bounceable { get; set; }

        public bool Testnet { get; set; }

        public string Addr { get; set; } = string.Empty;
    }
}
