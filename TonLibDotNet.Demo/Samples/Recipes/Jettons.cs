using Microsoft.Extensions.Logging;
using TonLibDotNet.Types;
using TonLibDotNet.Types.Msg;

namespace TonLibDotNet.Samples.Recipes
{
    public class Jettons : ISample
    {
        // some testnet jetton
        const string jettonMinterAddressTestnet = "EQBbX2khki4ynoYWgXqmc7_5Xlcley9luaHxoSE0-7R2whnK";

        // regular wallet, not jetton one!
        const string ownerWalletAddressTestnet = "EQAkEWzRLi1sw9AlaGDDzPvk2_F20hjpTjlvsjQqYawVmdT0";

        // jUSDT
        const string jettonMinterAddressMainnet = "EQBynBO23ywHy_CgarY9NK9FTz0yDsG82PtcbSTQgGoXwiuA";

        // regular wallet, not jetton one!
        const string ownerWalletAddressMainnet = "EQB3ncyBUTjZUA5EnFKR5_EnOMI9V1tTEAAPaiU71gc4TiUt";

        // regular wallet, not jetton one!
        const string receiverWalletWalletTestnet = "EQC403uCzev_-2g8fNfFPOgr5xOxCoTrCX2gp6OMK6YDtARk";

        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public Jettons(ITonClient tonClient, ILogger<Jettons> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run(bool inMainnet)
        {
            var jettonMinterAddress = inMainnet ? jettonMinterAddressMainnet : jettonMinterAddressTestnet;
            var ownerWalletAddress = inMainnet ? ownerWalletAddressMainnet : ownerWalletAddressTestnet;

            await tonClient.InitIfNeeded();

            var ownerJettonAddress = await TonRecipes.Jettons.GetWalletAddress(tonClient, jettonMinterAddress, ownerWalletAddress);
            logger.LogInformation("Jetton address for owner wallet {Wallet} is: {Address}", ownerWalletAddress, ownerJettonAddress);

            var wd = await TonRecipes.Jettons.GetWalletData(tonClient, ownerJettonAddress);
            logger.LogInformation("Info for Jetton address {Address}:", ownerJettonAddress);
            logger.LogInformation("  Balance:     {Value}", wd.balance);
            logger.LogInformation("  Owner:       {Value}", wd.ownerAddress);
            logger.LogInformation("  Jett.Master: {Value}", wd.jettonMinterAddress);
            ////logger.LogInformation("  Code:        \r\n{Value}", wd.jettonWalletCode.DumpCells());

            var jd = await TonRecipes.Jettons.GetJettonData(tonClient, jettonMinterAddress);
            logger.LogInformation("Info for Jetton Minter address {Address}:", jettonMinterAddress);
            logger.LogInformation("  Total supply:{Value}", jd.totalSupply);
            logger.LogInformation("  Mintable:    {Value}", jd.mintable);
            logger.LogInformation("  Admin:       {Value}", jd.adminAddress);
            ////logger.LogInformation("  Content:     \r\n{Value}", jd.jettonContent.DumpCells());
            ////logger.LogInformation("  Wallet code: \r\n{Value}", jd.jettonWalletCode.DumpCells());

            if (inMainnet)
            {
                logger.LogWarning("Jettons transfer sample in Mainnet is disabled for safety reasons. Switch to testnet in Program.cs and try again.");
            }
            else if (string.IsNullOrWhiteSpace(Program.TestMnemonic))
            {
                logger.LogWarning("Actual mnemonic is not set, sending jettons code is skipped. Put mnemonic phrase in Prograg.cs and try again.");
            }
            else
            {
                var msg = TonRecipes.Jettons.CreateTransferMessage(ownerJettonAddress, 12345, 1_000_000_000, receiverWalletWalletTestnet, ownerWalletAddress, null, 0.01M, null);

                var inputKey = await tonClient.ImportKey(new ExportedKey(Program.TestMnemonic.Split(' ').ToList()));

                var action = new ActionMsg(msg);
                var query = await tonClient.CreateQuery(new InputKeyRegular(inputKey), ownerWalletAddress, action, TimeSpan.FromMinutes(1));
                _ = await tonClient.QuerySend(query.Id);

                await tonClient.DeleteKey(inputKey);

                logger.LogInformation("Sent 1 jetton to {Dest}", receiverWalletWalletTestnet);
            }
        }
    }
}
