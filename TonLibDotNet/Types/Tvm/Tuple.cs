namespace TonLibDotNet.Types.Tvm
{
    [TLSchema("tvm.tuple elements:vector<tvm.StackEntry> = tvm.Tuple")]
    public class Tuple
    {
        public Tuple(List<StackEntry> elements)
        {
            Elements = elements;
        }

        public List<StackEntry> Elements { get; set; }
    }
}
