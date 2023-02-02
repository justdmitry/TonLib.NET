using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("getBip39Hints prefix:string = Bip39Hints")]
    public class GetBip39Hints : RequestBase<Bip39Hints>
    {
        public string? Prefix { get; set; }
    }
}
