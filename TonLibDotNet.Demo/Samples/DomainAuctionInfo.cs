using Microsoft.Extensions.Logging;
using TonLibDotNet.Types;
using TonLibDotNet.Types.Msg;
using TonLibDotNet.Types.Smc;

namespace TonLibDotNet.Samples
{
    public class DomainAuctionInfo
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public DomainAuctionInfo(ITonClient tonClient, ILogger<DomainAuctionInfo> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run(bool useMainnet)
        {
            if (!useMainnet)
            {
                logger.LogWarning("GetDomainAuctionInfo() demo in Testnet is disabled, because I don't know valid DNS Contract addresses");
                return;
            }

            await tonClient.InitIfNeeded();

            // Step 1: find some actual domain auction.
            // We need to check last IN transactions on ".ton DNS" address
            var dnsAddress = new AccountAddress("EQC3dNlesgVD8YbAazcauIrXBPfiVhMMr5YYk2in0Mtsz0Bz");
            var dnsState = await tonClient.GetAccountState(dnsAddress);
            var dnsTransactions = await tonClient.RawGetTransactions(dnsAddress, dnsState.LastTransactionId);
            var dnsTx = dnsTransactions.TransactionsList.FirstOrDefault(x => x.InMsg != null && x.InMsg.MsgData is DataText dt && !string.IsNullOrEmpty(dt.Text));
            if (dnsTx == null)
            {
                logger.LogError("Failed to find last Domain transaction");
                return;
            }

            TonUtils.Text.TryDecodeBase64((dnsTx.InMsg.MsgData as DataText).Text, out var domain);

            var domainAddress = dnsTx.OutMsgs[0].Destination;
            var bid = TonUtils.Coins.FromNano(dnsTx.OutMsgs[0].Value);

            logger.LogInformation("Last auction found: name {Name}.ton (address {Address}), last bid = {Value} TON, bidder is {Address}", domain, domainAddress.Value, bid, dnsTx.InMsg.Source.Value);

            // Step 2: Get some data from NFT
            var smc = await tonClient.SmcLoad(domainAddress);

            // Method 1: Call get_auction_info, it returns:
            // ;; MsgAddressInt max_bid_address
            // ;; Coins max_bid_amount
            // ;; int auction_end_time
            var smcgai = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_auction_info"));
            var adr = smcgai.Stack[0].ToTvmCell().ToBoc().RootCells[0].BeginRead().LoadAddressIntStd();
            var coins = long.Parse(smcgai.Stack[1].ToTvmNumberDecimal());
            var endTime = long.Parse(smcgai.Stack[2].ToTvmNumberDecimal());
            logger.LogInformation("Auction info (method 1): last bid = {Value} TON, bidder is {Address}, auction ends at {Time}", TonUtils.Coins.FromNano(coins), adr, DateTimeOffset.FromUnixTimeSeconds(endTime));

            // Method 2: Parse contract data
            //   structure is (from explorer source code)
            // ;; uint256 index
            // ;; MsgAddressInt collection_address
            // ;; MsgAddressInt owner_address
            // ;; cell content
            // ;; cell domain -e.g contains "alice"(without ending \0) for "alice.ton" domain
            // ;; cell auction - auction info
            // ;; int last_fill_up_time
            var data = await tonClient.SmcGetData(smc.Id);

            var slice = data.ToBoc().RootCells[0].BeginRead();

            // skip index
            slice.SkipBits(256);

            // load (and compare) collection address
            var ca = slice.LoadAddressIntStd();
            if (ca != dnsAddress.Value)
            {
                throw new Exception("Address mismatch. Something went wrong...");
            }

            // owner address (usually empty, as auction is in progress)
            slice.TryLoadAddressIntStd();

            // skip Content cell for now
            slice.LoadRef();

            // domain name is in second cell
            var domainName2 = System.Text.Encoding.ASCII.GetString(slice.LoadRef().Content);
            if (domainName2 != domain)
            {
                throw new Exception("Domain name mismatch. Something went wrong...");
            }

            var aucinfo = slice.LoadDict().BeginRead();
            var adr2 = aucinfo.LoadAddressIntStd();
            var coins2 = aucinfo.LoadCoins();
            var endTime2 = aucinfo.LoadLong();
            logger.LogInformation("Auction info (method 2): last bid = {Value} TON, bidder is {Address}, auction ends at {Time}", TonUtils.Coins.FromNano(coins2), adr2, DateTimeOffset.FromUnixTimeSeconds(endTime2));
        }
    }
}
