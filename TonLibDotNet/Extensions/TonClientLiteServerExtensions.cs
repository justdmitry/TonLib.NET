using TonLibDotNet.Requests.LiteServer;
using TonLibDotNet.Types.LiteServer;

namespace TonLibDotNet
{
    public static class TonClientLiteServerExtensions
    {
        /// <summary>
        /// Returns status information from current LiteServer.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L4587"/>
        public static Task<Info> LiteServerGetInfo(this ITonClient client)
        {
            return client.Execute(new GetInfo());
        }
    }
}
