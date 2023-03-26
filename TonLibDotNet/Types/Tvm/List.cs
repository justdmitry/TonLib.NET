namespace TonLibDotNet.Types.Tvm
{
    [TLSchema("tvm.list elements:vector<tvm.StackEntry> = tvm.List")]
    public class List
    {
        public List(List<StackEntry> elements)
        {
            Elements = elements;
        }

        public List<StackEntry> Elements { get; set; }
    }
}
