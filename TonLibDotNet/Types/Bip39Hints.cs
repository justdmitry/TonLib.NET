namespace TonLibDotNet.Types
{
    [TLSchema("bip39Hints words:vector<string> = Bip39Hints")]
    public class Bip39Hints : TypeBase
    {
        public List<string> Words { get; set; } = new();
    }
}
