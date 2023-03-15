namespace TonLibDotNet.Types.Dns
{
    [TLSchema("dns.resolved entries:vector<dns.entry> = dns.Resolved")]
    public class Resolved : TypeBase
    {
        public Resolved(List<Entry> entries)
        {
            Entries = entries ?? throw new ArgumentNullException(nameof(entries));
        }

        public List<Dns.Entry> Entries { get; set; }
    }
}
