using TonLibDotNet.Requests;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public static class TonClientExtensions
    {
        public static Task<FullAccountState> GetAccountState(this ITonClient client, AccountAddress accountAddress)
        {
            return client.Execute(new GetAccountState(accountAddress));
        }

        public static Task<FullAccountState> GetAccountState(this ITonClient client, string accountAddress)
        {
            return client.Execute(new GetAccountState(new AccountAddress(accountAddress)));
        }

        public static Task<Bip39Hints> GetBip39Hints(this ITonClient client, string? prefix = null)
        {
            return client.Execute(new GetBip39Hints() { Prefix = prefix });
        }

        public static Task<Types.Ton.BlockIdEx> Sync(this ITonClient client)
        {
            return client.Execute(new Sync());
        }
    }
}
