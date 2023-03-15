using System.Text.Json.Serialization;

namespace TonLibDotNet.Types.Dns
{
    [TLSchema("dns.entry name:string category:int256 entry:dns.EntryData = dns.Entry")]
    public class Entry : TypeBase
    {
        public Entry(string name, EntryData value)
        {
            Name = name;
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Name { get; set; }

        public string Category { get; set; } = Int256_AllZeroes;

        [JsonPropertyName("entry")]
        public EntryData Value { get; set; }
    }
}
