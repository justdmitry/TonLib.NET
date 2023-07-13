namespace TonLibDotNet.Types.Tvm
{
    [TLSchema("tvm.stackEntrySlice slice:tvm.slice = tvm.StackEntry")]
    public class StackEntrySlice : StackEntry
    {
        public StackEntrySlice(Slice slice)
        {
            Slice = slice ?? throw new ArgumentNullException(nameof(slice));
        }

        public StackEntrySlice(Cells.Boc boc)
        {
            Slice = new Slice(boc.SerializeToBase64());
        }

        public Slice Slice { get; set; }
    }
}
