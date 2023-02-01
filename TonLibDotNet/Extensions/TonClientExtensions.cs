using TonLibDotNet.Requests;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public static class TonClientExtensions
    {
        public static FullAccountState GetAccountState(this ITonClient client, AccountAddress accountAddress)
        {
            return client.Execute(new GetAccountState(accountAddress));
        }

        public static FullAccountState GetAccountState(this ITonClient client, string accountAddress)
        {
            return client.Execute(new GetAccountState(new AccountAddress(accountAddress)));
        }

        public static Types.Ton.BlockIdEx Sync(this ITonClient client)
        {
            return client.Execute(new Sync());
        }
    }
}
