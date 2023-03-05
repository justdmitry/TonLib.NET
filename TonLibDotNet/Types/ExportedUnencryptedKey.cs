namespace TonLibDotNet.Types
{
    [TLSchema("exportedUnencryptedKey data:secureBytes = ExportedUnencryptedKey")]
    public class ExportedUnencryptedKey : TypeBase
    {
        public string Data { get; set; } = string.Empty;
    }
}
