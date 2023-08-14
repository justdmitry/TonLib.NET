using Microsoft.Extensions.Logging;
using TonLibDotNet.Types;
using TonLibDotNet.Types.Msg;

namespace TonLibDotNet.Samples.Recipes
{
    public class NFTs : ISample
    {
        // Telemint (amounomous numbers +888)
        private const string CollectionAddressMainnet = "EQAOQdwdw8kGftJCSFgOErM1mBjYPe4DBPq8-AhF6vr9si5N";

        // Anonymous number *4035
        private const string NftAddressMainnet = "EQALmJq-Wzwiv-WkKan8CDPay-NNygbg2UbbiJ5w5FK055w7";

        // Harvest Haven
        private const string CollectionAddressTestnet = "EQCBKC58OU6FlbNVVpE-qkrVl1K5lc8tXWvFDyU7aqMBhfLB";

        private const string NftAddressTestnet = "EQBjoETJtep7ZnR4BVJLYhvWQE71v-UWsLFi3C1E3I4y6wth";

        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public NFTs(ITonClient tonClient, ILogger<NFTs> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run(bool inMainnet)
        {
            var collectionAddress = inMainnet ? CollectionAddressMainnet : CollectionAddressTestnet;
            var nftAddress = inMainnet ? NftAddressMainnet : NftAddressTestnet;

            await tonClient.InitIfNeeded();

            var cd = await TonRecipes.NFTs.GetCollectionData(tonClient, collectionAddress);
            logger.LogInformation("GetCollectionData() for NFT Collection {Address}:", collectionAddress);
            logger.LogInformation("  Collection nextItemIndex: {Value}", cd.nextItemIndex);
            logger.LogInformation("  Collection owner:         {Value}", cd.ownerAddress);
logger.LogInformation("Collectioncontent:\r\n{Value}",cd.collection_content.DumpCells());


            var nd = await TonRecipes.NFTs.GetNftData(tonClient, nftAddress);
            logger.LogInformation("GetNftData() for NFT Item {Address}:", nftAddress);
            logger.LogInformation("  Init?:              {Value}", nd.init);
            logger.LogInformation("  Index?:             {Value}", nd.index);
            logger.LogInformation("  Collection adr:     {Value}", nd.collectionAddress);
            logger.LogInformation("  Owner:              {Value}", nd.ownerAddress);
            logger.LogInformation("  Individual content:\r\n{Value}", nd.individualContent.DumpCells());

            var nftAdr2 = await TonRecipes.NFTs.GetNftAddressByIndex(tonClient, collectionAddress, nd.index);
            logger.LogInformation("GetNftAddressByIndex() with index (above) - must return original NFT address {Address}:", nftAddress);
            logger.LogInformation("  Returned address:   {Value}", nftAdr2);
            logger.LogInformation("  Valid?:             {Value}", nftAdr2 == nftAddress);

            var ic = await TonRecipes.NFTs.GetNftContent(tonClient, collectionAddress, nd.index, nd.individualContent);
            logger.LogInformation("GetNftContent() result:\r\n{Value}", ic.DumpCells());

            if (inMainnet)
            {
                logger.LogWarning("NFT transfer sample in Mainnet is disabled for safety reasons. Switch to testnet in Program.cs and try again.");
            }
            else if (string.IsNullOrWhiteSpace(Program.TestMnemonic))
            {
                logger.LogWarning("Actual mnemonic is not set, sending NFT code is skipped. Put mnemonic phrase in Program.cs and try again.");
            }
            else
            {
                var inputKey = await tonClient.ImportKey(new ExportedKey(Program.TestMnemonic.Split(' ').ToList()));

                // =====================
                // send get_static_data
                // =====================

                var msg = TonRecipes.NFTs.CreateGetStaticDataMessage(nftAddress, 12345);
                var action = new ActionMsg(msg);
                var query = await tonClient.CreateQuery(new InputKeyRegular(inputKey), Program.TestAddress, action, TimeSpan.FromMinutes(1));
                _ = await tonClient.QuerySend(query.Id);

                logger.LogInformation("get_static_data had been sent to {Address}", nftAddress);

                // =====================
                // send NFT to new owner
                // =====================

                // will sending to same owner wallet for demo purposes only.
                var destination = Program.TestAddress;

                msg = TonRecipes.NFTs.CreateTransferMessage(nftAddress, 12345, destination, Program.TestAddress, null, 0.01M, null);

                action = new ActionMsg(msg);
                query = await tonClient.CreateQuery(new InputKeyRegular(inputKey), Program.TestAddress, action, TimeSpan.FromMinutes(1));
                _ = await tonClient.QuerySend(query.Id);

                logger.LogInformation("NFT {Address} had been sent to {Address}", nftAddress, destination);

                // Cleanup
                await tonClient.DeleteKey(inputKey);
            }
        }
    }
}
