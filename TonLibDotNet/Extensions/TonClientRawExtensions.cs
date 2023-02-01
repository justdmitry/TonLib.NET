using TonLibDotNet.Requests.Raw;
using TonLibDotNet.Types.Raw;

namespace TonLibDotNet
{
    public static class TonClientRawExtensions
    {
        public static Task<FullAccountState> GetRawAccountState(this ITonClient client, Types.AccountAddress accountAddress)
        {
            return client.Execute(new GetAccountState(accountAddress));
        }

        public static Task<FullAccountState> GetRawAccountState(this ITonClient client, string accountAddress)
        {
            return client.Execute(new GetAccountState(new Types.AccountAddress(accountAddress)));
        }
    }
}
