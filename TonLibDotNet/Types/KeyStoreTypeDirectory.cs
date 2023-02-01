namespace TonLibDotNet.Types
{
    [TLSchema("keyStoreTypeDirectory directory:string = KeyStoreType")]
    public class KeyStoreTypeDirectory : KeyStoreType
    {
        public KeyStoreTypeDirectory(string directory)
        {
            Directory = directory;
        }

        public string Directory { get; set; }
    }
}
