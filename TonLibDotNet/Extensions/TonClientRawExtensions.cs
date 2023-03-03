using TonLibDotNet.Requests.Raw;
using TonLibDotNet.Types.Raw;

namespace TonLibDotNet
{
    public static class TonClientRawExtensions
    {
        /// <summary>
        /// Returns <see cref="FullAccountState"/> for specified account address.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="accountAddress">Address to get state for.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L2757"/>
        public static Task<FullAccountState> RawGetAccountState(this ITonClient client, Types.AccountAddress accountAddress)
        {
            ArgumentNullException.ThrowIfNull(accountAddress);
            if (string.IsNullOrEmpty(accountAddress.Value))
            {
                throw new ArgumentNullException(nameof(accountAddress));
            }

            return client.Execute(new GetAccountState(accountAddress));
        }

        /// <inheritdoc cref="RawGetAccountState(ITonClient, Types.AccountAddress)"/>
        public static Task<FullAccountState> RawGetAccountState(this ITonClient client, string accountAddress)
        {
            return RawGetAccountState(client, new Types.AccountAddress(accountAddress));
        }

        /// <summary>
        /// Get list of transactions for specified account, starting from specified one.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="accountAddress">Address to get transactions for.</param>
        /// <param name="fromTransactionId">Transaction to start from (this and previous will be returned).</param>
        /// <remarks>To get recent/last transactions - use LastTransactionId value from <see cref="RawGetAccountState(ITonClient, Types.AccountAddress)"/> or <see cref="TonClientExtensions.GetAccountState(ITonClient, Types.AccountAddress)"/> responses.</remarks>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L2784"/>
        public static Task<Transactions> RawGetTransactions(this ITonClient client, Types.AccountAddress accountAddress, Types.Internal.TransactionId fromTransactionId)
        {
            ArgumentNullException.ThrowIfNull(accountAddress);
            ArgumentNullException.ThrowIfNull(fromTransactionId);
            if (string.IsNullOrEmpty(accountAddress.Value))
            {
                throw new ArgumentNullException(nameof(accountAddress));
            }
            if (string.IsNullOrEmpty(fromTransactionId.Hash))
            {
                throw new ArgumentNullException(nameof(fromTransactionId));
            }

            return client.Execute(new GetTransactions(accountAddress, fromTransactionId));
        }

        /// <inheritdoc cref="RawGetTransactions(ITonClient, Types.AccountAddress, Types.Internal.TransactionId)"/>
        public static Task<Transactions> RawGetTransactions(this ITonClient client, string accountAddress, Types.Internal.TransactionId fromTransactionId)
        {
            return RawGetTransactions(client, new Types.AccountAddress(accountAddress), fromTransactionId);
        }
    }
}
