namespace TonLibDotNet.Types
{
    /// <remarks>
    /// TL Schema:
    /// <code>options config:config keystore_type:KeyStoreType = Options;</code>
    /// </remarks>
    public class Options : TypeBase
    {
        public Options()
        {
            TypeName = "options";
        }

        public Config Config { get; set; } = new Config();

        public KeyStoreType KeystoreType { get; set; } = new KeyStoreTypeInMemory();
    }
}
