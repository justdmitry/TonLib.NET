using TonLibDotNet.Requests.LiteServer;
using TonLibDotNet.Types.LiteServer;

namespace TonLibDotNet
{
    public static class TonClientLiteServerExtensions
    {
        public static Task<Info> LiteServerGetInfo(this ITonClient client)
        {
            return client.Execute(new GetInfo());
        }
    }
}
