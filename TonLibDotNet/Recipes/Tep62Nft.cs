using System.Globalization;
using System.Numerics;
using TonLibDotNet.Types.Msg;
using TonLibDotNet.Types.Smc;
using TonLibDotNet.Types.Tvm;
using TonLibDotNet.Utils;

namespace TonLibDotNet.Recipes
{
    /// <remarks>
    /// Based on <see href="https://github.com/ton-blockchain/TEPs/blob/master/text/0062-nft-standard.md">TEP 62: NFT Standard</see>
    ///   and <see href="https://github.com/ton-blockchain/token-contract/tree/main/nft/">Reference NFT implementation</see>.
    /// </remarks>
    public partial class Tep62Nft
    {
        public decimal DefaultAmount { get; set; } = 0.1M;

        public int DefaultSendMode { get; set; } = 1;

        /// <summary>
        /// From <see href="https://github.com/ton-blockchain/TEPs/blob/master/text/0062-nft-standard.md#1-transfer">0062-nft-standard.md</see>
        /// </summary>
        private const int OPTransfer = 0x5fcc3d14;

        /// <summary>
        /// From <see href="https://github.com/ton-blockchain/TEPs/blob/master/text/0062-nft-standard.md#2-get_static_data">0062-nft-standard.md</see>
        /// </summary>
        private const int OPGetStaticData = 0x2fcb26a2;

        public static readonly Tep62Nft Instance = new();

        /// <summary>
        /// Returns information about collection by calling 'get_collection_data' method on collection smart contract.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="collectionAddress">NFT Collection address.</param>
        /// <returns>
        /// <list type="table">
        /// <item><b>nextItemIndex</b> - the count of currently deployed NFT items in collection. Value <b>-1</b> means non-sequential indexing, and such collections should provide their own way for index generation / item enumeration;</item>
        /// <item><b>collection_content</b> - collection content in a format that complies with standard TEP-64;</item>
        /// <item><b>ownerAddress</b> - collection owner address, or null if no owner;</item>
        /// </list>
        /// </returns>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0062-nft-standard.md#get-methods-1">Get-methods description</seealso>
        /// <seealso href="https://github.com/ton-blockchain/token-contract/blob/main/nft/nft-collection.fc#L137">Reference implementation</seealso>
        public async Task<(byte[] nextItemIndex, Cells.Cell collection_content, string? ownerAddress)> GetCollectionData(ITonClient tonClient, string collectionAddress)
        {
            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(collectionAddress)).ConfigureAwait(false);

            // (int, cell, slice) get_collection_data()
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_collection_data")).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            TonLibNonZeroExitCodeException.ThrowIfNonZero(result.ExitCode);

            var nextIndex = result.Stack[0].ToBigIntegerBytes();
            var content = result.Stack[1].ToBoc().RootCells[0];
            var owner = result.Stack[2].ToBoc().RootCells[0].BeginRead().TryLoadAddressIntStd();

            return (nextIndex, content, owner);
        }

        /// <summary>
        /// Returns address of NFT Item smart contract by calling 'get_nft_address_by_index' method on collection smart contract.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="collectionAddress">NFT Collection address.</param>
        /// <param name="index">NFT Item index (usually 8 bytes).</param>
        /// <returns>Address of NFT Item.</returns>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0062-nft-standard.md#get-methods-1">Get-methods description</seealso>
        /// <seealso href="https://github.com/ton-blockchain/token-contract/blob/main/nft/nft-collection.fc#L143">Reference implementation</seealso>
        public async Task<string> GetNftAddressByIndex(ITonClient tonClient, string collectionAddress, byte[] index)
        {
            ArgumentNullException.ThrowIfNull(index);

            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(collectionAddress)).ConfigureAwait(false);

            // slice get_nft_address_by_index(int index)
            var stack = new List<StackEntry>()
            {
                new StackEntryNumber(new NumberDecimal(index)),
            };
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_nft_address_by_index"), stack).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            TonLibNonZeroExitCodeException.ThrowIfNonZero(result.ExitCode);

            return result.Stack[0].ToBoc().RootCells[0].BeginRead().LoadAddressIntStd();
        }

        /// <summary>
        /// Executes 'get_nft_data' method on NFT Item address contract, returns information about this NFT item.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="nftAddress">NFT address to obtain info for.</param>
        /// <returns>
        /// <list type="table">
        /// <item><i>init</i> - if true, then this NFT is fully initialized and ready for interaction;</item>
        /// <item><i>index</i> - numerical index of this NFT in the collection (for collection-less NFT - arbitrary but constant value);</item>
        /// <item><i>collectionAddress</i> - address of the smart contract of the collection to which this NFT belongs (for collection-less NFT this parameter should be null);</item>
        /// <item><i>ownerAddress</i> - address of the current owner of this NFT;</item>
        /// <item><i>individualContent</i> - if NFT has collection - individual NFT content in any format; if NFT has no collection - NFT content in format that complies with standard TEP-64;</item>
        /// </list>
        /// </returns>
        /// <remarks>NFT contract must be deployed and active (to execute get-method).</remarks>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0062-nft-standard.md#get-methods">Get-methods description</seealso>
        /// <seealso href="https://github.com/ton-blockchain/token-contract/blob/main/nft/nft-item.fc#L142">Reference implementation</seealso>
        public async Task<(bool init, byte[] index, string collectionAddress, string? ownerAddress, Cells.Boc individualContent)> GetNftData(ITonClient tonClient, string nftAddress)
        {
            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(nftAddress)).ConfigureAwait(false);

            // (int init?, int index, slice collection_address, slice owner_address, cell individual_content)
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_nft_data")).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            TonLibNonZeroExitCodeException.ThrowIfNonZero(result.ExitCode);

            var init = result.Stack[0].ToInt() != 0;
            var index = result.Stack[1].ToBigIntegerBytes();
            var collectionAddress = result.Stack[2].ToBoc().RootCells[0].BeginRead().LoadAddressIntStd();
            var ownerAddress = result.Stack[3].ToBoc().RootCells[0].BeginRead().LoadAddressIntStd();
            var individualContent = result.Stack[4].ToBoc();

            return (init, index, collectionAddress, ownerAddress, individualContent);
        }

        /// <inheritdoc cref="GetNftContent(ITonClient, string, byte[], Cells.Boc)"/>
        public Task<Cells.Boc> GetNftContent(ITonClient tonClient, string collectionAddress, byte[] index, Cells.Cell individualContent)
        {
            return GetNftContent(tonClient, collectionAddress, index, new Cells.Boc(individualContent));
        }

        /// <summary>
        /// Executes 'get_nft_content' method on Collection address contract and returns the full content of the NFT item.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="collectionAddress">NFT Collection address.</param>
        /// <param name="index">NFT Item index (usually 8 bytes).</param>
        /// <param name="individualContent">NFT address to obtain info for.</param>
        /// <returns>Cell with full content of the NFT item in format that complies with standard TEP-64.</returns>
        /// <remarks>
        /// <para>Collection contract must be deployed and active (to execute get-method).</para>
        /// <para>As an example, if an NFT item stores a metadata URI in its content, then a collection smart contract can store a domain (e.g. "https://site.org/"), and an NFT item smart contract in its content will store only the individual part of the link (e.g "kind-cobra"). In this example the get_nft_content method concatenates them and return "https://site.org/kind-cobra".</para>
        /// </remarks>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0062-nft-standard.md#get-methods-1">Get-methods description</seealso>
        /// <seealso href="https://github.com/ton-blockchain/token-contract/blob/main/nft/nft-collection.fc#L155">Reference implementation</seealso>
        public async Task<Cells.Boc> GetNftContent(ITonClient tonClient, string collectionAddress, byte[] index, Cells.Boc individualContent)
        {
            ArgumentNullException.ThrowIfNull(index);
            ArgumentNullException.ThrowIfNull(individualContent);

            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(collectionAddress)).ConfigureAwait(false);

            // cell get_nft_content(int index, cell individual_content)
            var stack = new List<StackEntry>()
            {
                new StackEntryNumber(new NumberDecimal(index)),
                new StackEntryCell(individualContent),
            };
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_nft_content"), stack).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            TonLibNonZeroExitCodeException.ThrowIfNonZero(result.ExitCode);

            var content = result.Stack[0].ToBoc();

            return content;
        }

        /// <summary>
        /// Creates message that will transfer NFT to new owner.
        /// </summary>
        /// <param name="nftAddress">NFT Item address to transfer.</param>
        /// <param name="queryId">Arbitrary request number.</param>
        /// <param name="newOwner">Address of the new owner of the NFT item.</param>
        /// <param name="responseDestination">Address where to send a response with confirmation of a successful transfer and the rest of the incoming message coins.</param>
        /// <param name="customPayload">Optional custom data.</param>
        /// <param name="forwardAmount">The amount of nanotons to be sent to the new owner.</param>
        /// <param name="forwardPayload">Optional custom data that should be sent to the new owner.</param>
        /// <returns>Constructed and ready-to-be-sent Message (by current owner of <paramref name="nftAddress"/>).</returns>
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0062-nft-standard.md#1-transfer">Transfer message in TEP</seealso>
        /// <seealso href="https://github.com/ton-blockchain/token-contract/blob/main/nft/nft-item.fc#L127">Reference implementation</seealso>
        public Message CreateTransferMessage(
            string nftAddress,
            ulong queryId,
            string newOwner,
            string responseDestination,
            Cells.Cell? customPayload,
            decimal forwardAmount,
            Cells.Cell? forwardPayload)
        {
            var body = new Cells.CellBuilder()
                .StoreUInt(OPTransfer)
                .StoreULong(queryId)
                .StoreAddressIntStd(newOwner)
                .StoreAddressIntStd(responseDestination)
                .StoreDict(customPayload)
                .StoreCoins(TonUtils.Coins.ToNano(forwardAmount))
                .StoreDict(forwardPayload)
                ;

            return new Message(new Types.AccountAddress(nftAddress))
            {
                Amount = TonUtils.Coins.ToNano(DefaultAmount),
                Data = new DataRaw(new Cells.Boc(body.Build()).SerializeToBase64(), string.Empty),
                SendMode = DefaultSendMode,
            };
        }

        /// <summary>
        /// Creates message to NFT Item that will instruct it to send back message with information about itself (index and collection address).
        /// </summary>
        /// <param name="nftAddress">NFT Item address to get info from.</param>
        /// <param name="queryId">Arbitrary request number.</param>
        /// <returns>Constructed and ready-to-be-sent Message.</returns>
        /// <remarks>
        /// <para>Upon receiving this message, NFT Item must send back message with the following layout and send-mode 64 (return msg amount except gas fees):
        /// <code>TL-B schema: report_static_data#8b771735 query_id:uint64 index:uint256 collection:MsgAddress = InternalMsgBody;</code>
        /// <list type="table">
        /// <item><i>query_id</i> - should be equal with request's query_id;</item>
        /// <item><i>index</i> - numerical index of this NFT in the collection (usually serial number of deployment);</item>
        /// <item><i>collection</i> - address of the smart contract of the collection to which this NFT belongs;</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0062-nft-standard.md#2-get_static_data">Get_static_data message in TEP</seealso>
        /// <seealso href="https://github.com/ton-blockchain/token-contract/blob/main/nft/nft-item.fc#L131">Reference implementation</seealso>
        public Message CreateGetStaticDataMessage(string nftAddress, ulong queryId)
        {
            var body = new Cells.CellBuilder()
                .StoreUInt(OPGetStaticData)
                .StoreULong(queryId)
                ;

            return new Message(new Types.AccountAddress(nftAddress))
            {
                Amount = TonUtils.Coins.ToNano(DefaultAmount),
                Data = new DataRaw(new Cells.Boc(body.Build()).SerializeToBase64(), string.Empty),
                SendMode = DefaultSendMode,
            };
        }
    }
}
