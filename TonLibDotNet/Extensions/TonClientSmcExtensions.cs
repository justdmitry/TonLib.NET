using TonLibDotNet.Requests.Smc;
using TonLibDotNet.Types;
using TonLibDotNet.Types.Internal;
using TonLibDotNet.Types.Smc;
using TonLibDotNet.Types.Tvm;

namespace TonLibDotNet
{
    public static class TonClientSmcExtensions
    {
        /// <summary>
        /// Removes previouly loaded smc info from memory.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="id">ID of previously loaded smc.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L3716" />
        public static Task<Ok> SmcForget(this ITonClient client, long id)
        {
            return client.Execute(new Forget(id));
        }

        /// <summary>
        /// Get 'code' data of smc.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="id">ID of previously loaded smc.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L3727" />
        public static Task<Cell> SmcGetCode(this ITonClient client, long id)
        {
            return client.Execute(new GetCode(id));
        }

        /// <summary>
        /// Get 'data' data of smc.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="id">ID of previously loaded smc.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L3739" />
        public static Task<Cell> SmcGetData(this ITonClient client, long id)
        {
            return client.Execute(new GetData(id));
        }

        /// <summary>
        /// Get 'state' data of smc.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="id">ID of previously loaded smc.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L3739" />
        public static Task<Cell> SmcGetState(this ITonClient client, long id)
        {
            return client.Execute(new GetState(id));
        }

        /// <summary>
        /// Load smc info into memory.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="accountAddress">Address to load smc for.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L3705" />
        public static Task<Info> SmcLoad(this ITonClient client, AccountAddress accountAddress)
        {
            ArgumentNullException.ThrowIfNull(accountAddress);
            if (string.IsNullOrEmpty(accountAddress.Value))
            {
                throw new ArgumentNullException(nameof(accountAddress));
            }

            return client.Execute(new Load(accountAddress));
        }

        /// <inheritdoc cref="SmcLoad(ITonClient, Types.AccountAddress)"/>
        public static Task<Info> SmcLoad(this ITonClient client, string accountAddress)
        {
            return SmcLoad(client, new Types.AccountAddress(accountAddress));
        }

        /// <summary>
        /// Load smc info into memory.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="accountAddress">Address to load smc for.</param>
        /// <param name="transactionId">TransactionId.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.03/tonlib/tonlib/TonlibClient.cpp#L4116" />
        public static Task<Info> SmcLoadByTransaction(this ITonClient client, AccountAddress accountAddress, TransactionId transactionId)
        {
            ArgumentNullException.ThrowIfNull(accountAddress);
            ArgumentNullException.ThrowIfNull(transactionId);
            if (string.IsNullOrEmpty(accountAddress.Value))
            {
                throw new ArgumentNullException(nameof(accountAddress));
            }

            return client.Execute(new LoadByTransaction(accountAddress, transactionId));
        }

        /// <summary>
        /// Executes GET method on smc.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="id">ID of previously loaded smc.</param>
        /// <param name="methodId">Method ID (<see cref="MethodIdName"/> or <see cref="MethodIdNumber"/>).</param>
        /// <param name="stack">Stack data.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.03/tonlib/tonlib/TonlibClient.cpp#L4361" />
        public static Task<RunResult> SmcRunGetMethod(this ITonClient client, long id, MethodId methodId, List<StackEntry>? stack = null)
        {
            ArgumentNullException.ThrowIfNull(methodId);

            return client.Execute(new RunGetMethod(id, methodId, stack));
        }
    }
}
