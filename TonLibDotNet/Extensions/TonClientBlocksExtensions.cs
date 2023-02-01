using TonLibDotNet.Requests.Blocks;
using TonLibDotNet.Types.Blocks;

namespace TonLibDotNet
{
    public static class TonClientBlocksExtensions
    {
        public static Task<MasterchainInfo> GetMasterchainInfo(this ITonClient client)
        {
            return client.Execute(new GetMasterchainInfo());
        }
    }
}
