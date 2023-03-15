namespace TonLibDotNet.Types.Dns
{
    [TLSchema("dns.entryDataText text:string = dns.EntryData")]
    public class EntryDataText : EntryData
    {
        public string Text { get; set; } = string.Empty;
    }
}
