using TonLibDotNet.Requests.Raw;
using TonLibDotNet.Types.Raw;

namespace TonLibDotNet
{
    public static class TonClientRawExtensions
    {
        public static Task<FullAccountState> RawGetAccountState(this ITonClient client, Types.AccountAddress accountAddress)
        {
            return client.Execute(new GetAccountState(accountAddress));
        }

        public static Task<FullAccountState> RawGetAccountState(this ITonClient client, string accountAddress)
        {
            return RawGetAccountState(client, new Types.AccountAddress(accountAddress));
        }

        public static Task<Transactions> RawGetTransactions(this ITonClient client, Types.AccountAddress accountAddress, Types.Internal.TransactionId fromTransactionId)
        {
            return client.Execute(new GetTransactions(accountAddress, fromTransactionId));
        }

        public static Task<Transactions> RawGetTransactions(this ITonClient client, string accountAddress, Types.Internal.TransactionId fromTransactionId)
        {
            return RawGetTransactions(client, new Types.AccountAddress(accountAddress), fromTransactionId);
        }
    }
}
