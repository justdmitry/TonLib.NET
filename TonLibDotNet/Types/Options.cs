namespace TonLibDotNet.Types
{
    [TLSchema("options config:config keystore_type:KeyStoreType = Options")]
    public class Options : TypeBase
    {
        public Config Config { get; set; } = new Config();

        public KeyStoreType KeystoreType { get; set; } = new KeyStoreTypeInMemory();
    }
}
