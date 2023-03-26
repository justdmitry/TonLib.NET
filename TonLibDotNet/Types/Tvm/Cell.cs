namespace TonLibDotNet.Types.Tvm
{
    [TLSchema("tvm.cell bytes:bytes = tvm.Cell")]
    public class Cell : TypeBase
    {
        public Cell(string bytes)
        {
            Bytes = bytes;
        }

        public string Bytes { get; set; }
    }
}
