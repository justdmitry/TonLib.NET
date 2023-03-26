namespace TonLibDotNet.Types.Tvm
{
    [TLSchema("tvm.stackEntrySlice slice:tvm.slice = tvm.StackEntry")]
    public class StackEntrySlice : StackEntry
    {
        public StackEntrySlice(Slice slice)
        {
            Slice = slice ?? throw new ArgumentNullException(nameof(slice));
        }

        public Slice Slice { get; set; }
    }
}
