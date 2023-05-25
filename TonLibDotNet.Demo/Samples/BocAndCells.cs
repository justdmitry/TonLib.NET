using System.Text;
using Microsoft.Extensions.Logging;
using TonLibDotNet.Cells;

namespace TonLibDotNet.Samples
{
    public class BocAndCells : ISample
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public BocAndCells(ITonClient tonClient, ILogger<BocAndCells> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run(bool inMainnet)
        {
            if (!inMainnet)
            {
                logger.LogWarning("BocAndCells() must be run in Mainnet to work correctly. Switch to testnet in Program.cs and try again.");
                return;
            }

            // If you know storage layout for some smartcontract,
            //    Than you can parse it "manually", without calling get-methods
            //    All 'expecting' data below is actual at time of writing this test (April 2023).

            // https://tonapi.io/account/EQDendoireMDFMufOUzkqNpFIay83GnjV2tgGMbA64wA3siV
            const string adr = "EQDendoireMDFMufOUzkqNpFIay83GnjV2tgGMbA64wA3siV"; // tonapi.ton domain NFT

            var info = await tonClient.SmcLoad(adr);

            // This is "RAW DATA" of this smartcontracts, as you can see it in explorer
            var data = await tonClient.SmcGetData(info.Id);
            var boc = Boc.ParseFromBase64(data.Bytes);

            // Here is dump fo you to compare, if you want.
            logger.LogInformation("BOC (compare with raw data that explorer shows to you):\r\n{Text}", boc.DumpCells());

            // Here is storage "structure" (I found this in smartcontract code on explorer)
            //
            // uint256 index
            // MsgAddressInt collection_address
            // MsgAddressInt owner_address
            // cell content
            // cell domain -e.g contains "alice"(without ending \0) for "alice.ton" domain
            // cell auction - auction info
            // int last_fill_up_time

            // Domain name is in second ref cell
            var domain = Encoding.ASCII.GetString(boc.RootCells[0].Refs[1].Content);
            logger.LogInformation("Domain (expecting 'tonapi'): {Value}", domain);

            // TO obtain other data, we need to read cell as slice and read according to data types
            var rootSlice = boc.RootCells[0].BeginRead();

            // skip over index
            rootSlice.SkipBits(256);

            // get collection address
            var collectionAddress = rootSlice.LoadAddressIntStd();
            logger.LogInformation("Collection (expecting 'EQC3dNlesgVD8YbAazcauIrXBPfiVhMMr5YYk2in0Mtsz0Bz'): {Value}", collectionAddress);

            // get owner address
            var ownerAddress = rootSlice.LoadAddressIntStd();
            logger.LogInformation("Owner (expecting 'EQCNdbNc28ZrcE3AKGDqK18-NFbcSzhTGaRPeEqnMIJiQsl_'): {Value}", ownerAddress);

            rootSlice.SkipBits(1); // skip bit about auction data

            var lastFillUp = rootSlice.LoadLong();
            logger.LogInformation("Last fill-up: {Value}", DateTimeOffset.FromUnixTimeSeconds(lastFillUp));
            logger.LogInformation("Expires (compare with dns.ton.org): {Value}", DateTimeOffset.FromUnixTimeSeconds(lastFillUp).AddYears(1));

        }
    }
}
