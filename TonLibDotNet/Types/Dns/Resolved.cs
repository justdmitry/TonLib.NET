using System.Text.Json.Serialization;

namespace TonLibDotNet.Types.Dns
{
    [TLSchema("dns.resolved entries:vector<dns.entry> = dns.Resolved")]
    public class Resolved : TypeBase
    {
        [JsonConstructor]
        public Resolved(List<Entry> entries)
        {
            Entries = entries ?? throw new ArgumentNullException(nameof(entries));
        }

        public Resolved(params Entry[] entries)
        {
            Entries = entries.ToList();
        }

        public List<Dns.Entry> Entries { get; set; }
    }
}
