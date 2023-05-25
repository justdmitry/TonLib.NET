using Microsoft.Extensions.Logging;
using TonLibDotNet.Types.Msg;
using TonLibDotNet.Types.Smc;

namespace TonLibDotNet.Samples
{
    public class DomainAuctionInfo : ISample
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public DomainAuctionInfo(ITonClient tonClient, ILogger<DomainAuctionInfo> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run(bool inMainnet)
        {
            if (!inMainnet)
            {
                logger.LogWarning("GetDomainAuctionInfo() sample in Testnet is disabled, because I don't know valid DNS Contract addresses. Switch to mainnet in Program.cs and try again.");
                return;
            }

            await tonClient.InitIfNeeded();

            // First, we need to find some actual domain auction.
            var (domainName, domainAddress) = await FindDomainOnAuction();
            if (string.IsNullOrEmpty(domainName))
            {
                logger.LogError("Failed to find last Domain-on-Auction transaction");
                return;
            }

            // Method 1: Call get_auction_info manually, it returns:
            // ;; MsgAddressInt max_bid_address
            // ;; Coins max_bid_amount
            // ;; int auction_end_time
            var smc = await tonClient.SmcLoad(domainAddress);
            var smcgai = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_auction_info"));
            var adr = smcgai.Stack[0].ToTvmCell().ToBoc().RootCells[0].BeginRead().LoadAddressIntStd();
            var coins = long.Parse(smcgai.Stack[1].ToTvmNumberDecimal());
            var endTime = long.Parse(smcgai.Stack[2].ToTvmNumberDecimal());
            logger.LogInformation("Auction info (method 1): last bid = {Value} TON, bidder is {Address}, auction ends at {Time}", TonUtils.Coins.FromNano(coins), adr, DateTimeOffset.FromUnixTimeSeconds(endTime));

            // Method 1-bis: use TonRecipes to call 'get_auction_info' method.
            var ai = await TonRecipes.RootDns.GetAuctionInfo(tonClient, domainAddress);
            logger.LogInformation("Auction info (method 1-bis): last bid = {Value} TON, bidder is {Address}, auction ends at {Time}", ai!.MaxBidAmount, ai.MaxBidAddress, ai.AuctionEndTime);

            // Method 2: Use TonRecipes to parse all DNS Item data.
            var di = await TonRecipes.RootDns.GetAllInfo(tonClient, domainName);
            logger.LogInformation("Auction info (method 2): last bid = {Value} TON, bidder is {Address}, auction ends at {Time}", di.AuctionInfo!.MaxBidAmount, di.AuctionInfo.MaxBidAddress, di.AuctionInfo.AuctionEndTime);
        }

        private async Task<(string name, string address)> FindDomainOnAuction()
        {
            // We need to check last IN transactions on ".ton DNS" address
            var rootDnsAddress = "EQC3dNlesgVD8YbAazcauIrXBPfiVhMMr5YYk2in0Mtsz0Bz";

            var rootDnsState = await tonClient.GetAccountState(rootDnsAddress);
            var rootDnsTransactions = await tonClient.RawGetTransactions(rootDnsAddress, rootDnsState.LastTransactionId);

            // Look for transaction with text comment
            var dnsTx = rootDnsTransactions.TransactionsList.FirstOrDefault(x => x.InMsg != null && x.InMsg.MsgData is DataText dt && !string.IsNullOrEmpty(dt.Text));
            if (dnsTx == null)
            {
                return (string.Empty, string.Empty);
            }

            TonUtils.Text.TryDecodeBase64((dnsTx.InMsg!.MsgData as DataText)!.Text, out var domain);
            domain += ".ton";

            var domainAddress = dnsTx.OutMsgs![0].Destination;
            var bid = TonUtils.Coins.FromNano(dnsTx.OutMsgs[0].Value);

            logger.LogInformation("Last auction found: domain '{Name}' (address {Address}), last bid = {Value} TON, bidder is {Address}", domain, domainAddress.Value, bid, dnsTx.InMsg.Source.Value);

            return (domain, domainAddress.Value);
        }
    }
}
