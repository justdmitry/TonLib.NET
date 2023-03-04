namespace TonLibDotNet.Types
{
    [TLSchema("key public_key:string secret:secureBytes = Key")]
    public class Key : TypeBase
    {
        public string PublicKey { get; set; } = string.Empty;

        public string Secret { get; set; } = string.Empty;
    }
}
