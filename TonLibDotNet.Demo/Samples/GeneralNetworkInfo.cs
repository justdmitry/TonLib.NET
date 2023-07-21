using Microsoft.Extensions.Logging;
using TonLibDotNet.Utils;

namespace TonLibDotNet.Samples
{
    public class GeneralNetworkInfo : ISample
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public GeneralNetworkInfo(ITonClient tonClient, ILogger<GeneralNetworkInfo> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run(bool inMainnet)
        {
            await tonClient.InitIfNeeded();

            var lsi = await tonClient.LiteServerGetInfo();
            logger.LogInformation("Server time: {Now}", lsi.Now);

            var mi = await tonClient.GetMasterchainInfo();
            logger.LogInformation("Last block: shard = {Shard}, seqno = {Seqno}", mi.Last.Shard, mi.Last.Seqno);

            var cp = await tonClient.GetConfigParam(4);
            logger.LogInformation("Config Param 04 (DNS Root Resolver): {Value}", AddressValidator.MakeAddress(0xFF, cp.Config.ToBoc().RootCells[0].BeginRead().LoadBitsToBytes(256)));

            cp = await tonClient.GetConfigParam(14);

            var slice = cp.Config.ToBoc().RootCells[0].BeginRead();
            slice.SkipBits(8); // 0x6b
            var masterChain = slice.LoadCoins();
            var baseChain = slice.LoadCoins();
            slice.EndRead();

            logger.LogInformation(
                "Config Param 14 (New block reward): masterchain {Value} TON, basechain {Value} TON",
                TonUtils.Coins.FromNano(masterChain),
                TonUtils.Coins.FromNano(baseChain));
        }
    }
}
