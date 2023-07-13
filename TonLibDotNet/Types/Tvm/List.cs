using System.Text.Json.Serialization;

namespace TonLibDotNet.Types.Tvm
{
    [TLSchema("tvm.list elements:vector<tvm.StackEntry> = tvm.List")]
    public class List
    {
        [JsonConstructor]
        public List(List<StackEntry> elements)
        {
            Elements = elements ?? throw new ArgumentNullException(nameof(elements));
        }

        public List(params StackEntry[] elements)
        {
            Elements = elements.ToList();
        }

        public List<StackEntry> Elements { get; set; }
    }
}
