using System.Text.Json.Serialization;

namespace TonLibDotNet.Types.Tvm
{
    [TLSchema("tvm.stackEntryCell cell:tvm.cell = tvm.StackEntry")]
    public class StackEntryCell : StackEntry
    {
        [JsonConstructor]
        public StackEntryCell(Cell cell)
        {
            Cell = cell ?? throw new ArgumentNullException(nameof(cell));
        }

        public StackEntryCell(Cells.Boc boc)
        {
            Cell = new Cell(boc.SerializeToBase64());
        }

        public Cell Cell { get; set; }
    }
}
