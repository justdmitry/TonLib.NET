namespace TonLibDotNet.Types.Dns
{
    [TLSchema("dns.entryDataNextResolver resolver:AccountAddress = dns.EntryData")]
    public class EntryDataNextResolver : EntryData
    {
        public EntryDataNextResolver(AccountAddress resolver)
        {
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public AccountAddress Resolver { get; set; }
    }
}
