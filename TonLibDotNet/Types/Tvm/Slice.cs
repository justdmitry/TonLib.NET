namespace TonLibDotNet.Types.Tvm
{
    [TLSchema("tvm.slice bytes:bytes = tvm.Slice")]
    public class Slice : TypeBase
    {
        public Slice(string bytes)
        {
            Bytes = bytes;
        }

        public string Bytes { get; set; }
    }
}
