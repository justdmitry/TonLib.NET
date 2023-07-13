using System.Text;
using Microsoft.Extensions.Logging;
using TonLibDotNet.Types.Smc;
using TonLibDotNet.Types;

namespace TonLibDotNet.Samples
{
    public class ReadInfoFromSmartContracts : ISample
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public ReadInfoFromSmartContracts(ITonClient tonClient, ILogger<ReadInfoFromSmartContracts> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Run(bool inMainnet)
        {
            return inMainnet ? RunOnMainnet() : RunOnTestnet();
        }

        protected async Task RunOnTestnet()
        {
            await tonClient.InitIfNeeded();

            // https://ton-community.github.io/tutorials/02-contract/
            const string adr = "EQAHI1vGuw7d4WG-CtfDrWqEPNtmUuKjKFEFeJmZaqqfWTvW";

            var info = await tonClient.SmcLoad(new AccountAddress(adr));
            _ = await tonClient.SmcGetCode(info.Id);
            _ = await tonClient.SmcGetState(info.Id);
            _ = await tonClient.SmcGetData(info.Id);
            _ = await tonClient.SmcRunGetMethod(info.Id, new MethodIdName("counter"));

            _ = await tonClient.SmcForget(info.Id);
        }

        protected async Task RunOnMainnet()
        {
            await tonClient.InitIfNeeded();

            // https://tonapi.io/account/EQDendoireMDFMufOUzkqNpFIay83GnjV2tgGMbA64wA3siV
            const string adr = "EQDendoireMDFMufOUzkqNpFIay83GnjV2tgGMbA64wA3siV"; // tonapi.ton domain NFT

            var info = await tonClient.SmcLoad(adr);

            var rr = await tonClient.SmcRunGetMethod(info.Id, new MethodIdName("get_domain"));
            var boc = rr.Stack[0].ToBoc();
            logger.LogInformation("Domain (expecting 'tonapi'): {Value}", Encoding.ASCII.GetString(boc.RootCells[0].Content));

            // Check contract source code in expolorer to understand what get_nft_data() returns
            rr = await tonClient.SmcRunGetMethod(info.Id, new MethodIdName("get_nft_data"));
            boc = rr.Stack[3].ToBoc();

            var slice = boc.RootCells[0].BeginRead();
            var ads = slice.LoadAddressIntStd();
            slice.EndRead();

            logger.LogInformation("Owner (expecting 'EQCNdbNc28ZrcE3AKGDqK18-NFbcSzhTGaRPeEqnMIJiQsl_'): {Value}", ads);

            _ = await tonClient.SmcForget(info.Id);
        }
    }
}
