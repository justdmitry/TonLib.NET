namespace TonLibDotNet.Types.Tvm
{
    [TLSchema("tvm.stackEntryList list:tvm.List = tvm.StackEntry")]
    public class StackEntryList : StackEntry
    {
        public StackEntryList(List list)
        {
            List = list ?? throw new ArgumentNullException(nameof(list));
        }

        public List List { get; set; }
    }
}
