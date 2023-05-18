using Microsoft.Extensions.Logging;
using TonLibDotNet.Utils;

namespace TonLibDotNet.Samples
{
    public class AccountBalanceAndTransactions
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        private const string account = "EQCD39VS5jcptHL8vMjEXrzGaRcCVYto7HUn4bpAOg8xqB2N"; // TON Foundation wallet

        public AccountBalanceAndTransactions(ITonClient tonClient, ILogger<AccountBalanceAndTransactions> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run()
        {
            await tonClient.InitIfNeeded();

            // Get info about account from LiteServer
            var uaa = await tonClient.UnpackAccountAddress(account);
            logger.LogInformation("Address info 1: Bounceable={Value}, Testnet={Value}, Workchain={Value}, Bytes={Value}", uaa.WorkchainId, uaa.Bounceable, uaa.Testnet, uaa.Addr);

            // Or, parse account locally without interacting with LiteServer
            _ = AddressValidator.TryParseAddress(account, out var workchainId, out var accountId, out var bounceable, out var testnetOnly, out var urlSafe);
            logger.LogInformation("Address info 2: Bounceable={Value}, Testnet={Value}, Workchain={Value}, Bytes={Value}", workchainId, bounceable, testnetOnly, Convert.ToBase64String(accountId));

            var ast = await tonClient.GetAccountState(account);
            logger.LogInformation("Acc info via GetAccountState(): balance = {Value} nanoton or {Value} TON", ast.Balance, TonUtils.Coins.FromNano(ast.Balance));

            var rast = await tonClient.RawGetAccountState(account);
            logger.LogInformation("Acc info via RawGetAccountState(): balance = {Value} nanoton or {Value} TON", rast.Balance, TonUtils.Coins.FromNano(rast.Balance));

            var txs = await tonClient.RawGetTransactions(account, rast.LastTransactionId);
            foreach (var item in txs.TransactionsList)
            {
                if (item.InMsg?.Value > 0)
                {
                    logger.LogInformation("TX {Id}: {Value} ({Value} TON) from {Address}", item.TransactionId.Hash, item.InMsg.Value, TonUtils.Coins.FromNano(item.InMsg.Value), item.InMsg.Source.Value);
                }

                if (item.OutMsgs != null)
                {
                    foreach(var msg in item.OutMsgs)
                    {
                        logger.LogInformation("TX {Id}: {Value} ({Value} TON) to {Address}", item.TransactionId.Hash, msg.Value, TonUtils.Coins.FromNano(msg.Value), msg.Destination.Value);
                    }
                }
            }
        }
    }
}
