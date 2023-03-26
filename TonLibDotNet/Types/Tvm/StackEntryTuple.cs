namespace TonLibDotNet.Types.Tvm
{
    [TLSchema("tvm.stackEntryTuple tuple:tvm.Tuple = tvm.StackEntry")]
    public class StackEntryTuple : StackEntry
    {
        public StackEntryTuple(Tuple tuple)
        {
            Tuple = tuple ?? throw new ArgumentNullException(nameof(tuple));
        }

        public Tuple Tuple { get; set; }
    }
}
