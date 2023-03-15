namespace TonLibDotNet.Types.Dns
{
    [TLSchema("dns.entryDataAdnlAddress adnl_address:AdnlAddress = dns.EntryData")]
    public class EntryDataAdnlAddress : EntryData
    {
        public EntryDataAdnlAddress(AdnlAddress adnlAddress)
        {
            AdnlAddress = adnlAddress ?? throw new ArgumentNullException(nameof(adnlAddress));
        }

        public AdnlAddress AdnlAddress { get; set; }
    }
}
