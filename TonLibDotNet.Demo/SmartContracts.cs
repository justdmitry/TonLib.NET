using System.Text;
using Microsoft.Extensions.Logging;
using TonLibDotNet.Types.Smc;
using TonLibDotNet.Types;
using TonLibDotNet.Cells;
using TonLibDotNet.Types.Tvm;

namespace TonLibDotNet
{
    public class SmartContracts
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public SmartContracts(ITonClient tonClient, ILogger<SmartContracts> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Run(bool mainnet)
        {
            return mainnet ? RunOnMainnet() : RunOnTestnet();
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

            // var libs = await tonClient.Execute(new GetLibraries("1234567890"));

            _ = await tonClient.SmcForget(info.Id);
        }

        protected async Task RunOnMainnet()
        {
            await tonClient.InitIfNeeded();

            // https://tonapi.io/account/EQDendoireMDFMufOUzkqNpFIay83GnjV2tgGMbA64wA3siV
            const string adr = "EQDendoireMDFMufOUzkqNpFIay83GnjV2tgGMbA64wA3siV"; // tonapi.ton domain NFT

            var info = await tonClient.SmcLoad(adr);

            var rr = await tonClient.SmcRunGetMethod(info.Id, new MethodIdName("get_domain"));
            if (Boc.TryParseFromBase64(((StackEntryCell)rr.Stack[0]).Cell.Bytes, out var boc))
            {
                logger.LogInformation("Domain (expecting 'tonapi'): {Value}", Encoding.ASCII.GetString(boc.RootCells[0].Content));
            }

            rr = await tonClient.SmcRunGetMethod(info.Id, new MethodIdName("get_nft_data"));
            if (Boc.TryParseFromBase64(((StackEntryCell)rr.Stack[3]).Cell.Bytes, out boc))
            {
                var slice = boc.RootCells[0].BeginRead();
                var ads = slice.LoadAddressIntStd();
                slice.EndRead();

                logger.LogInformation("Owner (expecting 'EQCNdbNc28ZrcE3AKGDqK18-NFbcSzhTGaRPeEqnMIJiQsl_'): {Value}", ads);
            }

            _ = await tonClient.SmcForget(info.Id);
        }
    }
}
