using TonLibDotNet.Requests;
using TonLibDotNet.Types;
using TonLibDotNet.Utils;

namespace TonLibDotNet
{
    [TLSchema("hello.world = hello.World")]
    public class ExtensibilityDemoRequest : RequestBase<Ok>
    {
        public string Continent { get; set; } = string.Empty;
    }
}
