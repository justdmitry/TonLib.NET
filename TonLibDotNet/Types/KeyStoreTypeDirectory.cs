namespace TonLibDotNet.Types
{
    /// <remarks>
    /// TL Schema:
    /// <code>keyStoreTypeDirectory directory:string = KeyStoreType;</code>
    /// </remarks>
    public class KeyStoreTypeDirectory : KeyStoreType
    {
        public KeyStoreTypeDirectory(string directory)
        {
            TypeName = "keyStoreTypeDirectory";
            Directory = directory;
        }

        public string Directory { get; set; }
    }
}
