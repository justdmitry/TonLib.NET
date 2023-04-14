using System.Text;
using Microsoft.Extensions.Logging;
using TonLibDotNet.Cells;

namespace TonLibDotNet
{
    public class BocAndCells
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public BocAndCells(ITonClient tonClient, ILogger<BocAndCells> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run()
        {
            // If you know storage layout for some smartcontract,
            //    Than you can parse it "manually", without calling get-methods

            // https://tonapi.io/account/EQDendoireMDFMufOUzkqNpFIay83GnjV2tgGMbA64wA3siV
            const string adr = "EQDendoireMDFMufOUzkqNpFIay83GnjV2tgGMbA64wA3siV"; // tonapi.ton domain NFT

            var info = await tonClient.SmcLoad(adr);
            var data = await tonClient.SmcGetData(info.Id);
            var boc = Boc.ParseFromBase64(data.Bytes);
            logger.LogInformation("BOC:\r\n{Text}", boc.DumpCells());

            var domain = Encoding.ASCII.GetString(boc.RootCells[0].Refs[1].Content);
            logger.LogInformation("Domain (expecting 'tonapi'): {Value}", domain);

            // Storage (from smartcontract code on explorer)
            //
            // uint256 index
            // MsgAddressInt collection_address
            // MsgAddressInt owner_address
            // cell content
            // cell domain -e.g contains "alice"(without ending \0) for "alice.ton" domain
            // cell auction - auction info
            // int last_fill_up_time
            //
            var rootSlice = boc.RootCells[0].BeginRead();
            rootSlice.SkipBits(256);
            var collectionAddress = rootSlice.LoadAddressIntStd();
            logger.LogInformation("Collection (expecting 'EQC3dNlesgVD8YbAazcauIrXBPfiVhMMr5YYk2in0Mtsz0Bz'): {Value}", collectionAddress);

            var ownerAddress = rootSlice.LoadAddressIntStd();
            logger.LogInformation("Owner (expecting 'EQCNdbNc28ZrcE3AKGDqK18-NFbcSzhTGaRPeEqnMIJiQsl_'): {Value}", ownerAddress);
        }
    }
}
