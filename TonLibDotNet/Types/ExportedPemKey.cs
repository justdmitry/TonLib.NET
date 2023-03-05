namespace TonLibDotNet.Types
{
    [TLSchema("exportedPemKey pem:secureString = ExportedPemKey")]
    public class ExportedPemKey : TypeBase
    {
        public string Pem { get; set; } = string.Empty;
    }
}
