namespace TonLibDotNet.Types.Dns
{
    [TLSchema("dns.entryDataUnknown bytes:bytes = dns.EntryData")]
    public class EntryDataUnknown : EntryData
    {
        public string Bytes { get; set; } = string.Empty;
    }
}
