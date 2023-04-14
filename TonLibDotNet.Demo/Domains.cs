using Microsoft.Extensions.Logging;
using TonLibDotNet.Types.Dns;

namespace TonLibDotNet
{
    public class Domains
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public Domains(ITonClient tonClient, ILogger<Domains> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run()
        {
            await tonClient.InitIfNeeded();

            // Option 1: use non-empty TTL
            var res = await tonClient.DnsResolve("toncenter.ton", ttl: 9);
            var adnl1 = (res.Entries[0].Value as EntryDataAdnlAddress)?.AdnlAddress.Value;

            // Option 2: Use zero TTL and recurse yourself
            res = await tonClient.DnsResolve("ton.", null, null, null);
            // Now we have NFT Collection (EQC3dNlesgVD8YbAazcauIrXBPfiVhMMr5YYk2in0Mtsz0Bz) in 'NextResolver'
            // Let's ask it for next part
            res = await tonClient.DnsResolve("toncenter.", (res.Entries[0].Value as EntryDataNextResolver).Resolver, null, null);
            // Now we have NFT itself (Contract Type = Domain, toncenter.ton, EQAiIsvar4OYBn8BGBf9flfin6tl5poBx4MgJe4CQJYasy51) in 'NextResolver'
            var nftAccountAddress = (res.Entries[0].Value as EntryDataNextResolver).Resolver;
            // Let's ask it about actual ADNL of this domain
            res = await tonClient.DnsResolve(".", nftAccountAddress, null, null);
            // And now we have ADNL address in answer
            var adnl2 = (res.Entries[0].Value as EntryDataAdnlAddress)?.AdnlAddress.Value;

            logger.LogInformation("Results:\r\nADNL1 {Val}\r\nADNL2 {Val}", adnl1, adnl2);

            // Some experiments. Try TTL=1
            res = await tonClient.DnsResolve("toncenter.ton", null, 1);
            // We now have again 'EQAiIsvar4OYBn8BGBf9flfin6tl5poBx4MgJe4CQJYasy51' in 'NextResolver' !
            // You see? TTL is a number of 'recursive iterations' to 'NextResolvers' that is performed by LiteServer (or tonlib?) itself.
            // Checking with TTL=2, we should receive final ADNL record:
            res = await tonClient.DnsResolve("toncenter.ton", null, 2);
            var adnl3 = (res.Entries[0].Value as EntryDataAdnlAddress)?.AdnlAddress.Value; // Yes, we do!

            // Unfortunately, asking for account state of NFT itself returns raw.AccountState, not Dns.AccountState, I don't know why :(
            _ = await tonClient.GetAccountState("EQAiIsvar4OYBn8BGBf9flfin6tl5poBx4MgJe4CQJYasy51");
            // But check RunMainnetSmcDemo() to know how to get owner of this NFT!
        }
    }
}
