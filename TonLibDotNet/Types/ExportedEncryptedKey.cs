namespace TonLibDotNet.Types
{
    [TLSchema("exportedEncryptedKey data:secureBytes = ExportedEncryptedKey")]
    public class ExportedEncryptedKey : TypeBase
    {
        public string Data { get; set; } = string.Empty;
    }
}
