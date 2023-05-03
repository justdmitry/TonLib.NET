using Microsoft.Extensions.Logging;

namespace TonLibDotNet.Samples
{
    public class GeneralNetworkInfo
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public GeneralNetworkInfo(ITonClient tonClient, ILogger<GeneralNetworkInfo> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run()
        {
            await tonClient.InitIfNeeded();

            var lsi = await tonClient.LiteServerGetInfo();
            logger.LogInformation("Server time: {Now}", lsi.Now);

            var mi = await tonClient.GetMasterchainInfo();
            logger.LogInformation("Last block: shard = {Shard}, seqno = {Seqno}", mi.Last.Shard, mi.Last.Seqno);
        }
    }
}
