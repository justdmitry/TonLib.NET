using System.Text.Json.Serialization;

namespace TonLibDotNet.Types.Tvm
{
    [TLSchema("tvm.tuple elements:vector<tvm.StackEntry> = tvm.Tuple")]
    public class Tuple
    {
        [JsonConstructor]
        public Tuple(List<StackEntry> elements)
        {
            Elements = elements ?? throw new ArgumentNullException(nameof(elements));
        }

        public Tuple(params StackEntry[] elements)
        {
            Elements = elements.ToList();
        }

        public List<StackEntry> Elements { get; set; }
    }
}
