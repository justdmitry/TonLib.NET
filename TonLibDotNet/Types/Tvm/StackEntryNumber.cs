namespace TonLibDotNet.Types.Tvm
{
    [TLSchema("tvm.stackEntryNumber number:tvm.Number = tvm.StackEntry")]
    public class StackEntryNumber : StackEntry
    {
        public StackEntryNumber(Number number)
        {
            Number = number ?? throw new ArgumentNullException(nameof(number));
        }

        public Number Number { get; set; }
    }
}
