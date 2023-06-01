using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace TonLibDotNet.Samples.Recipes
{
    public class RootDnsGetAllInfo : ISample
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public RootDnsGetAllInfo(ITonClient tonClient, ILogger<RootDnsGetAllInfo> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run(bool inMainnet)
        {
            if (!inMainnet)
            {
                logger.LogWarning("Recipes_GetDomainInfo() sample in Testnet is disabled, because I don't know valid DNS Contract addresses. Switch to mainnet in Program.cs and try again.");
                return;
            }

            await tonClient.InitIfNeeded();

            var domain = "foundation.ton";

            var di = await TonRecipes.RootDns.GetAllInfoByName(tonClient, domain);

            logger.LogInformation(
                "TonRecipes info for '{Domain}':\r\n{Info}",
                domain,
                JsonSerializer.Serialize(di, new JsonSerializerOptions() { WriteIndented = true }));
        }
    }
}
