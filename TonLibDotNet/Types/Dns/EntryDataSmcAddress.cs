namespace TonLibDotNet.Types.Dns
{
    [TLSchema("dns.entryDataSmcAddress smc_address:AccountAddress = dns.EntryData")]
    public class EntryDataSmcAddress : EntryData
    {
        public EntryDataSmcAddress(AccountAddress smcAddress)
        {
            SmcAddress = smcAddress ?? throw new ArgumentNullException(nameof(smcAddress));
        }

        public AccountAddress SmcAddress { get; set; }
    }
}
