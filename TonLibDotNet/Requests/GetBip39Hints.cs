using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("getBip39Hints prefix:string = Bip39Hints")]
    public class GetBip39Hints : RequestBase<Bip39Hints>
    {
        public GetBip39Hints()
        {
            // Proof: https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L1852
            this.IsStatic = true;
        }

        public string? Prefix { get; set; }
    }
}
