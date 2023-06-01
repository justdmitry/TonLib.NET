using Microsoft.Extensions.Logging;
using TonLibDotNet.Samples.Recipes;
using TonLibDotNet.Types.Dns;

namespace TonLibDotNet.Samples
{
    public class ResolveDomains : ISample
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        // Values below may change over time. Verify them using explorers as first troubleshooting step.
        private const string DomainName = "toncenter";
        private const string DomainNameFull = "toncenter.ton";

        public ResolveDomains(ITonClient tonClient, ILogger<ResolveDomains> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run(bool inMainnet)
        {
            if (!inMainnet)
            {
                logger.LogWarning("ResolveDomains() sample in Testnet is disabled, because I don't know valid DNS Contract addresses. Switch to mainnet in Program.cs and try again.");
                return;
            }

            await tonClient.InitIfNeeded();

            // Method 1: use non-empty TTL
            var res1 = await tonClient.DnsResolve(DomainNameFull, ttl: 9);
            var adnl1 = (res1.Entries[0].Value as EntryDataAdnlAddress)?.AdnlAddress.Value;

            // Method 2: Use zero TTL and recurse yourself
            var res2 = await tonClient.DnsResolve("ton", null, null, null);
            // Now we have NFT Collection (EQC3dNlesgVD8YbAazcauIrXBPfiVhMMr5YYk2in0Mtsz0Bz) in 'NextResolver'
            // Let's ask it for next part
            res2 = await tonClient.DnsResolve(DomainName, (res2.Entries[0].Value as EntryDataNextResolver).Resolver, null, null);
            // Now we have NFT itself (Contract Type = Domain, toncenter.ton, EQAiIsvar4OYBn8BGBf9flfin6tl5poBx4MgJe4CQJYasy51) in 'NextResolver'
            var nftAccountAddress = (res2.Entries[0].Value as EntryDataNextResolver).Resolver;
            // Let's ask it about actual ADNL of this domain
            res2 = await tonClient.DnsResolve(".", nftAccountAddress, null, null);
            // And now we have ADNL address in answer
            var adnl2 = (res2.Entries[0].Value as EntryDataAdnlAddress)?.AdnlAddress.Value;

            logger.LogInformation("Results:\r\nADNL (method 1): {Val}\r\nADNL (method 2): {Val}", adnl1, adnl2);

            // Some experiments. Try TTL=1
            var res3 = await tonClient.DnsResolve(DomainNameFull, null, 1);
            // We now have again 'EQAiIsvar4OYBn8BGBf9flfin6tl5poBx4MgJe4CQJYasy51' in 'NextResolver' !
            // You see? TTL is a number of 'recursive iterations' to 'NextResolvers' that is performed by LiteServer itself.
            // Checking with TTL=2, we should receive final ADNL record:
            var res4 = await tonClient.DnsResolve(DomainNameFull, null, 2);
            var adnl4 = (res4.Entries[0].Value as EntryDataAdnlAddress)?.AdnlAddress.Value; // Yes, we do!

            // and get records
            var entries = await TonRecipes.RootDns.GetEntries(tonClient, nftAccountAddress.Value);
            logger.LogInformation("Wallet:        {Value}", entries.Wallet);
            logger.LogInformation("Storage:       {Value}", entries.Storage);
            logger.LogInformation("Site (ADNL):   {Value}", entries.SiteToAdnl);
            logger.LogInformation("Site (Storage):{Value}", entries.SiteToStorage);
            logger.LogInformation("Next Resolver: {Value}", entries.DnsNextResolver);

            // Method 3: Use TonRecipes and recieve all info with one call
            logger.LogWarning("Also, check {Name}!", nameof(RootDnsGetAllInfo));
        }
    }
}
