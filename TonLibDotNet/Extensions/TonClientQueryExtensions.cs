using TonLibDotNet.Requests;
using TonLibDotNet.Requests.Query;
using TonLibDotNet.Types;
using TonLibDotNet.Types.Query;

namespace TonLibDotNet
{
    public static class TonClientQueryExtensions
    {
        /// <summary>
        /// Creates query for specific action (without sending it).
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="privateKey">Key to sign.</param>
        /// <param name="address">Target address.</param>
        /// <param name="action">Action to send.</param>
        /// <param name="timeout">TTL (in seconds)</param>
        /// <param name="initialAccountState">
        /// Desired wallet type if you need to deploy <b>sender</b> contract (<see cref="Types.Wallet.V3InitialAccountState"/> or <see cref="Types.Wallet.HighloadV1InitialAccountState"/> or <see cref="Types.Wallet.HighloadV2InitialAccountState"/>).
        /// </param>
        /// <seealso href="https://ton.org/docs/develop/dapps/asset-processing/#deploying-wallet"/>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L3530" />
        public static Task<Info> CreateQuery(this ITonClient client, InputKey privateKey, AccountAddress address, Types.Action action, TimeSpan timeout = default, InitialAccountState? initialAccountState = null)
        {
            ArgumentNullException.ThrowIfNull(nameof(privateKey));
            ArgumentNullException.ThrowIfNull(nameof(address));
            ArgumentNullException.ThrowIfNull(nameof(action));

            return client.Execute(new CreateQuery(privateKey, address, action) { Timeout = (long)timeout.TotalSeconds, InitialAccountState = initialAccountState });
        }

        /// <inheritdoc cref="CreateQuery(ITonClient, InputKey, AccountAddress, Types.Action, TimeSpan, InitialAccountState?)"/>
        public static Task<Info> CreateQuery(this ITonClient client, InputKey privateKey, string address, Types.Action action, TimeSpan timeout = default, InitialAccountState? initialAccountState = null)
        {
            return CreateQuery(client, privateKey, new AccountAddress(address), action, timeout, initialAccountState);
        }

        /// <summary>
        /// Send query to blockchain.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="id">Id of previously created query.</param>
        /// <seealso href="https://ton.org/docs/develop/dapps/asset-processing/#deploying-wallet"/>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L3658" />
        public static Task<Ok> QuerySend(this ITonClient client, long id)
        {
            return client.Execute(new Send(id));
        }

        /// <summary>
        /// Remove query without sending, cleanup memory.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="id">Id of previously created query.</param>
        /// <remarks>It seems that this method does NOTHING (does not delete query), at least in tag v2023.01</remarks>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L3675" />
        public static Task<Ok> QueryForget(this ITonClient client, long id)
        {
            return client.Execute(new Forget(id));
        }

        /// <summary>
        /// Get estimated fees for query.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="id">Id of previously created query.</param>
        /// <param name="ignoreChksig">Some boolean param, feel free to make PR with its description.</param>
        /// <seealso href="https://ton.org/docs/develop/smart-contracts/fees"/>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L3644" />
        public static Task<Types.Query.Fees> QueryEstimateFees(this ITonClient client, long id, bool ignoreChksig = false)
        {
            return client.Execute(new EstimateFees() { Id = id, IgnoreChksig = ignoreChksig });
        }

        /// <summary>
        /// Get information about query by Id.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="id">Id of previously created query.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L3624" />
        public static Task<Info> QueryGetInfo(this ITonClient client, long id)
        {
            return client.Execute(new GetInfo(id));
        }
    }
}
