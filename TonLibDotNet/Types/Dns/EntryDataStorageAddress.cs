namespace TonLibDotNet.Types.Dns
{
    [TLSchema("dns.entryDataStorageAddress bag_id:int256 = dns.EntryData")]
    public class EntryDataStorageAddress : EntryData
    {
        public string BagId { get; set; } = string.Empty;
    }
}
