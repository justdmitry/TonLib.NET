using System.Globalization;
using System.Numerics;
using TonLibDotNet.Cells;
using TonLibDotNet.Types.Msg;
using TonLibDotNet.Types;
using TonLibDotNet.Types.Smc;
using TonLibDotNet.Utils;
using System.Diagnostics.CodeAnalysis;

namespace TonLibDotNet.Recipes
{
    /// <remarks>
    /// Based on <see href="https://github.com/ton-blockchain/TEPs/blob/master/text/0074-jettons-standard.md">TEP 74: Fungible tokens (Jettons) standard</see>
    ///   and <see href="https://github.com/ton-blockchain/token-contract/">Tokens Smart Contracts</see>.
    /// </remarks>
    public partial class Tep74Jettons
    {
        public decimal DefaultAmount { get; set; } = 0.1M;

        public int DefaultSendMode { get; set; } = 1;

        /// <summary>
        /// From <see href="https://github.com/ton-blockchain/token-contract/blob/main/ft/op-codes.fc">op-codes.fc</see>
        /// </summary>
        private const int OPTransfer = 0xf8a7ea5;

        /// <summary>
        /// From <see href="https://github.com/ton-blockchain/token-contract/blob/main/ft/op-codes.fc">op-codes.fc</see>
        /// </summary>
        private const int OPBurn = 0x595f07bc;

        /// <summary>
        /// From <see href="https://github.com/ton-blockchain/token-contract/blob/main/ft/op-codes.fc">op-codes.fc</see>
        /// </summary>
        private const int OPTransferNotification = 0x7362d09c;

        public static readonly Tep74Jettons Instance = new();

        /// <summary>
        /// Executes 'get_wallet_address' method on Jetton Minter contract, returns jetton address for specified owner (user) wallet address.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="jettonMinterAddress">Jetton Minter contract address.</param>
        /// <param name="ownerAddress">Owner (user) wallet address to get jetton address for.</param>
        /// <returns>Jetton address for specified Jetton and specified owner (user) wallet address.</returns>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/ton-blockchain/token-contract/blob/main/ft/jetton-minter.fc#L114">Source of 'get_wallet_address' method.</seealso>
        public async Task<string> GetWalletAddress(ITonClient tonClient, string jettonMinterAddress, string ownerAddress)
        {
            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(jettonMinterAddress)).ConfigureAwait(false);

            // slice get_wallet_address(slice owner_address)
            var stack = new List<Types.Tvm.StackEntry>
            {
                new Types.Tvm.StackEntrySlice(new CellBuilder().StoreAddressIntStd(ownerAddress).Build().ToBoc()),
            };

            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_wallet_address"), stack).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            TonLibNonZeroExitCodeException.ThrowIfNonZero(result.ExitCode);

            return result.Stack[0].ToBoc().RootCells[0].BeginRead().LoadAddressIntStd();
        }

        /// <summary>
        /// Executes 'get_wallet_data' method on Jetton address contract, returns information about this jetton address.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="jettonAddress">Jetton address to obtain info for.</param>
        /// <returns>
        /// <list type="table">
        /// <item><i>balance</i> - amount of jettons on wallet</item>
        /// <item><i>ownerAddress</i> - address of wallet owner</item>
        /// <item><i>jettonMinterAddress</i> - address of Jetton master-address</item>
        /// <item><i>jettonWalletCode</i> - code of this wallet</item>
        /// </list>
        /// </returns>
        /// <remarks>Jetton contract must be deployed and active (to execute get-method).</remarks>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/ton-blockchain/token-contract/blob/main/ft/jetton-wallet.fc#L246">Source of 'get_wallet_address' method.</seealso>
        public async Task<(BigInteger balance, string ownerAddress, string jettonMinterAddress, Boc jettonWalletCode)> GetWalletData(ITonClient tonClient, string jettonAddress)
        {
            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(jettonAddress)).ConfigureAwait(false);

            // (int balance:Coins, slice owner_address:MsgAddressInt, slice jetton_master_address:MsgAddressInt, cell jetton_wallet_code:^Cell)
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_wallet_data")).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            TonLibNonZeroExitCodeException.ThrowIfNonZero(result.ExitCode);

            var balance = result.Stack[0].ToBigInteger();
            var owner = result.Stack[1].ToBoc().RootCells[0].BeginRead().LoadAddressIntStd();
            var minter = result.Stack[2].ToBoc().RootCells[0].BeginRead().LoadAddressIntStd();
            var code = result.Stack[3].ToBoc();

            return (balance, owner, minter, code);
        }

        /// <summary>
        /// Executes 'get_jetton_data' method on Jetton Minter contract, returns information about this jetton.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="jettonMinterAddress">Jetton Minter address to obtain info for.</param>
        /// <returns>
        /// <list type="table">
        /// <item><i>totalSupply</i> - the total number of issues jettons</item>
        /// <item><i>mintable</i> - flag which indicates whether number of jettons can increase</item>
        /// <item><i>adminAddress</i> - address of smart-contract which control Jetton</item>
        /// <item><i>jettonContent</i> - data in accordance to Token Data Standard #64</item>
        /// <item><i>jettonWalletCode</i> - code of wallet for that jetton</item>
        /// </list>
        /// </returns>
        /// <remarks>Jetton contract must be deployed and active (to execute get-method).</remarks>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/ton-blockchain/token-contract/blob/main/ft/jetton-wallet.fc#L246">Source of 'get_wallet_address' method.</seealso>
        public async Task<(BigInteger totalSupply, bool mintable, string? adminAddress, Boc jettonContent, Boc jettonWalletCode)> GetJettonData(ITonClient tonClient, string jettonMinterAddress)
        {
            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(jettonMinterAddress)).ConfigureAwait(false);

            // (int total_supply, int mintable, slice admin_address, cell jetton_content, cell jetton_wallet_code)
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_jetton_data")).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            TonLibNonZeroExitCodeException.ThrowIfNonZero(result.ExitCode);

            var totalSupply = result.Stack[0].ToBigInteger();
            var mintable = result.Stack[1].ToInt() != 0;
            var adminAddress = result.Stack[2].ToBoc().RootCells[0].BeginRead().TryLoadAddressIntStd();
            var jettonContent = result.Stack[3].ToBoc();
            var jettonWalletCode = result.Stack[4].ToBoc();

            return (totalSupply, mintable, adminAddress, jettonContent, jettonWalletCode);
        }

        /// <summary>
        /// Creates body cell that should be used to transfer jettons from source to destination.
        /// </summary>
        /// <param name="queryId">Arbitrary request number.</param>
        /// <param name="amount">Amount of transferred jettons <b>in elementary units</b>.</param>
        /// <param name="destination">Address of the new owner of the jettons (user main-wallet address, not his jetton address).</param>
        /// <param name="responseDestination">Address where to send a response with confirmation of a successful transfer and the rest of the incoming message Toncoins.</param>
        /// <param name="customPayload">Optional custom data (which is used by either sender or receiver jetton wallet for inner logic).</param>
        /// <param name="forwardTonAmount">The amount of nanotons to be sent to the destination address.</param>
        /// <param name="forwardPayload">Optional custom data that should be sent to the destination address.</param>
        /// <returns>Constructed Cell (body of message to send).</returns>
        /// <remarks>
        /// <para>Your Jetton wallet address must already be deployed and active, and contain enough jettons to send.</para>
        /// </remarks>
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0074-jettons-standard.md#1-transfer">Transfer message in TEP</seealso>
        public Cell CreateTransferCell(
            ulong queryId,
            BigInteger amount,
            string destination,
            string responseDestination,
            Cell? customPayload,
            decimal forwardTonAmount,
            Cell? forwardPayload)
        {
            return new CellBuilder()
                .StoreUInt(OPTransfer)
                .StoreULong(queryId)
                .StoreCoins(amount)
                .StoreAddressIntStd(destination)
                .StoreAddressIntStd(responseDestination)
                .StoreDict(customPayload)
                .StoreCoins(TonUtils.Coins.ToNano(forwardTonAmount))
                .StoreDict(forwardPayload)
                .Build();
        }

        /// <summary>
        /// Creates message that will transfer jettons from source to destination.
        /// </summary>
        /// <param name="sourceJettonAddress">Jetton wallet address to send coins from (use <see cref="GetWalletAddress">GetWalletAddress</see> if needed).</param>
        /// <param name="queryId">Arbitrary request number.</param>
        /// <param name="amount">Amount of transferred jettons <b>in elementary units</b>.</param>
        /// <param name="destination">Address of the new owner of the jettons (user main-wallet address, not his jetton address).</param>
        /// <param name="responseDestination">Address where to send a response with confirmation of a successful transfer and the rest of the incoming message Toncoins.</param>
        /// <param name="customPayload">Optional custom data (which is used by either sender or receiver jetton wallet for inner logic).</param>
        /// <param name="forwardTonAmount">The amount of nanotons to be sent to the destination address.</param>
        /// <param name="forwardPayload">Optional custom data that should be sent to the destination address.</param>
        /// <returns>Constructed and ready-to-be-sent Message (by editor/owner of <paramref name="sourceJettonAddress"/>).</returns>
        /// <remarks>
        /// <para>Your Jetton wallet address must already be deployed and active, and contain enough jettons to send.</para>
        /// </remarks>
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0074-jettons-standard.md#1-transfer">Transfer message in TEP</seealso>
        public Message CreateTransferMessage(
            string sourceJettonAddress,
            ulong queryId,
            BigInteger amount,
            string destination,
            string responseDestination,
            Cell? customPayload,
            decimal forwardTonAmount,
            Cell? forwardPayload)
        {
            var body = CreateTransferCell(queryId, amount, destination, responseDestination, customPayload, forwardTonAmount, forwardPayload);

            return new Message(new AccountAddress(sourceJettonAddress))
            {
                Amount = TonUtils.Coins.ToNano(DefaultAmount),
                Data = new DataRaw(new Boc(body).SerializeToBase64(), string.Empty),
                SendMode = DefaultSendMode,
            };
        }

        /// <summary>
        /// Creates body cell that should be used to burn specified amount of jettons.
        /// </summary>
        /// <param name="queryId">Arbitrary request number.</param>
        /// <param name="amount">Amount of jettons to burn <b>in elementary units</b>.</param>
        /// <param name="responseDestination">Address where to send a response with confirmation of a successful transfer and the rest of the incoming message Toncoins.</param>
        /// <param name="customPayload">Optional custom data (which is used by either sender or receiver jetton wallet for inner logic).</param>
        /// <returns>Constructed Cell (body of message to send).</returns>
        /// <remarks>
        /// <para>Your Jetton wallet address must already be deployed and active, and contain enough jettons to send.</para>
        /// </remarks>
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0074-jettons-standard.md#2-burn">Burn message in TEP</seealso>
        public Cell CreateBurnCell(
            ulong queryId,
            BigInteger amount,
            string responseDestination,
            Cell? customPayload)
        {
            return new CellBuilder()
                .StoreUInt(OPBurn)
                .StoreULong(queryId)
                .StoreCoins(amount)
                .StoreAddressIntStd(responseDestination)
                .StoreDict(customPayload)
                .Build();
        }

        /// <summary>
        /// Creates message that will burn specified amount of jettons.
        /// </summary>
        /// <param name="sourceJettonAddress">Jetton wallet address to send coins from (use <see cref="GetWalletAddress">GetWalletAddress</see> if needed).</param>
        /// <param name="queryId">Arbitrary request number.</param>
        /// <param name="amount">Amount of jettons to burn <b>in elementary units</b>.</param>
        /// <param name="responseDestination">Address where to send a response with confirmation of a successful transfer and the rest of the incoming message Toncoins.</param>
        /// <param name="customPayload">Optional custom data (which is used by either sender or receiver jetton wallet for inner logic).</param>
        /// <returns>Constructed and ready-to-be-sent Message (by editor/owner of <paramref name="sourceJettonAddress"/>).</returns>
        /// <remarks>
        /// <para>Your Jetton wallet address must already be deployed and active, and contain enough jettons to send.</para>
        /// </remarks>
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0074-jettons-standard.md#2-burn">Burn message in TEP</seealso>
        public Message CreateBurnMessage(
            string sourceJettonAddress,
            ulong queryId,
            BigInteger amount,
            string responseDestination,
            Cell? customPayload)
        {
            var body = CreateBurnCell(queryId, amount, responseDestination, customPayload);

            return new Message(new AccountAddress(sourceJettonAddress))
            {
                Amount = TonUtils.Coins.ToNano(DefaultAmount),
                Data = new DataRaw(new Boc(body).SerializeToBase64(), string.Empty),
                SendMode = DefaultSendMode,
            };
        }

        /// <summary>
        /// Checks if incoming message is a Transfer Notification, and parses it.
        /// </summary>
        /// <param name="msg">Incoming transaction message.</param>
        /// <param name="jettonWalletAddress">Address of jetton wallet who sent coins (compare with your known jetton wallet addresses to know what jetton has arrived).</param>
        /// <param name="queryId">Request number.</param>
        /// <param name="userWalletAddress">Address of user (master wallet) who sent jettons.</param>
        /// <param name="amount">Amount of jettons to burn <b>in elementary units</b>.</param>
        /// <param name="forwardPayload">Optional custom data.</param>
        /// <returns>True when message is a jetton transfer notification message and it has been parsed succesfully, false otherwise.</returns>
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0074-jettons-standard.md#1-transfer">Transfer message in TEP</seealso>
        public bool TryParseJettonTransferNotification(
            TonLibDotNet.Types.Raw.Message? msg,
            [NotNullWhen(true)] out string? jettonWalletAddress,
            [NotNullWhen(true)] out long? queryId,
            [NotNullWhen(true)] out string? userWalletAddress,
            [NotNullWhen(true)] out BigInteger? amount,
            out Cell? forwardPayload)
        {
            jettonWalletAddress = default;
            queryId = default;
            userWalletAddress = default;
            amount = default;
            forwardPayload = default;

            if (msg is null || msg.MsgData is not DataRaw data || string.IsNullOrWhiteSpace(data.Body))
            {
                return false;
            }

            jettonWalletAddress = msg.Source.Value;

            var slice = Boc.ParseFromBase64(data.Body).RootCells[0].BeginRead();

            if (!slice.TryCanLoad(32))
            {
                return false;
            }

            var op = slice.LoadInt(32);
            if (op != OPTransferNotification)
            {
                return false;
            }

            queryId = slice.LoadLong(64);
            amount = slice.LoadCoinsToBigInt();
            userWalletAddress = slice.LoadAddressIntStd();

            if (slice.TryCanLoadRef())
            {
                forwardPayload = slice.LoadRef();
            }

            return true;
        }
    }
}
