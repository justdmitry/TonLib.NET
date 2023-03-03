using TonLibDotNet.Requests;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public static class TonClientExtensions
    {
        /// <summary>
        /// Returns <see cref="FullAccountState"/> for specified account address.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="accountAddress">Address to get state for.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L2869" />
        public static Task<FullAccountState> GetAccountState(this ITonClient client, AccountAddress accountAddress)
        {
            ArgumentNullException.ThrowIfNull(accountAddress);
            if (string.IsNullOrEmpty(accountAddress.Value))
            {
                throw new ArgumentNullException(nameof(accountAddress));
            }

            return client.Execute(new GetAccountState(accountAddress));
        }

        /// <inheritdoc cref="GetAccountState(ITonClient, AccountAddress)"/>
        public static Task<FullAccountState> GetAccountState(this ITonClient client, string accountAddress)
        {
            return GetAccountState(client, new AccountAddress(accountAddress));
        }

        /// <summary>
        /// Returns mnemonic words that match prefix (or all words if prefix is empty).
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="prefix">Prefix to match (optional).</param>
        /// <remarks>Executes as static (without LiteServer call).</remarks>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L2183"/>
        public static Task<Bip39Hints> GetBip39Hints(this ITonClient client, string? prefix = null)
        {
            return client.Execute(new GetBip39Hints() { Prefix = prefix });
        }

        /// <summary>
        /// Converts <see cref="UnpackedAccountAddress">unpacked</see> account address to <see cref="AccountAddress">packed</see> one.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="accountAddress">Unpacked account address.</param>
        /// <remarks>Executes as static (without LiteServer call).</remarks>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L2167"/>
        public static Task<AccountAddress> PackAccountAddress(this ITonClient client, UnpackedAccountAddress accountAddress)
        {
            ArgumentNullException.ThrowIfNull(accountAddress);
            if (string.IsNullOrEmpty(accountAddress.Addr))
            {
                throw new ArgumentNullException(nameof(accountAddress));
            }

            return client.Execute(new PackAccountAddress(accountAddress));
        }

        /// <summary>
        /// Syncs local data with LiteServer, return last <see cref="Types.Ton.BlockIdEx">block</see> data.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L4263"/>
        public static Task<Types.Ton.BlockIdEx> Sync(this ITonClient client)
        {
            return client.Execute(new Sync());
        }

        /// <summary>
        /// Converts <see cref="AccountAddress">packed</see> account address to <see cref="UnpackedAccountAddress">unpacked</see> one.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="accountAddress">Packed account address.</param>
        /// <remarks>Executes as static (without LiteServer call).</remarks>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L2155"/>
        public static Task<UnpackedAccountAddress> UnpackAccountAddress(this ITonClient client, string accountAddress)
        {
            if (string.IsNullOrEmpty(accountAddress))
            {
                throw new ArgumentNullException(nameof(accountAddress));
            }

            return client.Execute(new UnpackAccountAddress(accountAddress));
        }

        /// <inheritdoc cref="UnpackAccountAddress(ITonClient, string)"/>
        public static Task<UnpackedAccountAddress> UnpackAccountAddress(this ITonClient client, AccountAddress accountAddress)
        {
            ArgumentNullException.ThrowIfNull(accountAddress);

            return UnpackAccountAddress(client, accountAddress.Value);
        }
    }
}
