using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace TonLibDotNet.Samples.Recipes
{
    public class TelemintGetAllInfo : ISample
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public TelemintGetAllInfo(ITonClient tonClient, ILogger<TelemintGetAllInfo> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run(bool inMainnet)
        {
            if (!inMainnet)
            {
                logger.LogWarning("Recipes_GetTelemintInfo() sample in Testnet is disabled, because I don't know valid DNS Contract addresses. Switch to mainnet in Program.cs and try again.");
                return;
            }

            await tonClient.InitIfNeeded();

            var domain = "dolboeb.t.me";

            var ti = await TonRecipes.TelegramUsernames.GetAllInfoByName(tonClient, domain);

            // Output as JSON because I'm too lazy to write separate lines for each field.
            logger.LogInformation(
                "Info about '{Domain}':\r\n{Json}",
                domain,
                JsonSerializer.Serialize(ti, new JsonSerializerOptions() { WriteIndented = true }));

            var number = "+888 0000 8888";
            ti = await TonRecipes.TelegramNumbers.GetAllInfoByName(tonClient, number);
            logger.LogInformation(
                "Info about '{Domain}':\r\n{Json}",
                number,
                JsonSerializer.Serialize(ti, new JsonSerializerOptions() { WriteIndented = true }));
        }
    }
}
