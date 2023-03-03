using TonLibDotNet.Requests.Blocks;
using TonLibDotNet.Types.Blocks;

namespace TonLibDotNet
{
    public static class TonClientBlocksExtensions
    {
        /// <summary>
        /// Returns <see cref="MasterchainInfo"/> from current LiteServer.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L4670"/>
        public static Task<MasterchainInfo> GetMasterchainInfo(this ITonClient client)
        {
            return client.Execute(new GetMasterchainInfo());
        }
    }
}
