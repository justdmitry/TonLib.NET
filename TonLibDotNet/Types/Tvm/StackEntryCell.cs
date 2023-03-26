namespace TonLibDotNet.Types.Tvm
{
    [TLSchema("tvm.stackEntryCell cell:tvm.cell = tvm.StackEntry")]
    public class StackEntryCell : StackEntry
    {
        public StackEntryCell(Cell cell)
        {
            Cell = cell ?? throw new ArgumentNullException(nameof(cell));
        }

        public Cell Cell { get; set; }
    }
}
