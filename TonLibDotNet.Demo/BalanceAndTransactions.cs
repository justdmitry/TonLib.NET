using Microsoft.Extensions.Logging;
using TonLibDotNet.Utils;

namespace TonLibDotNet
{
    public class BalanceAndTransactions
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public BalanceAndTransactions(ITonClient tonClient, ILogger<BalanceAndTransactions> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run()
        {
            await tonClient.InitIfNeeded();

            var lsi = await tonClient.LiteServerGetInfo();
            logger.LogInformation("Server time: {Now}", lsi.Now);

            var mi = await tonClient.GetMasterchainInfo();
            logger.LogInformation("Last block: shard = {Shard}, seqno = {Seqno}", mi.Last.Shard, mi.Last.Seqno);

            var account = "EQCJTkhd1W2wztkVNp_dsKBpv2SIoUWoIyzI7mQrbSrj_NSh"; // TON Diamonds

            // Get info about account from LiteServer
            var uaa = await tonClient.UnpackAccountAddress(account);
            logger.LogInformation("Address info 1: Bounceable={1}, Testnet={2}, Workchain={3}, Bytes={4}", uaa.WorkchainId, uaa.Bounceable, uaa.Testnet, uaa.Addr);

            // Or, parse account locally without interacting with LiteServer
            _ = AddressValidator.TryParseAddress(account, out var workchainId, out var accountId, out var bounceable, out var testnetOnly, out var urlSafe);
            logger.LogInformation("Address info 2: Bounceable={1}, Testnet={2}, Workchain={3}, Bytes={4}", workchainId, bounceable, testnetOnly, accountId);

            var ast = await tonClient.GetAccountState(account);
            logger.LogInformation("Acc info 1: balance = {Value}", ast.Balance);

            var rast = await tonClient.RawGetAccountState(account);
            logger.LogInformation("Acc info 2: balance = {Value}", rast.Balance);

            var txs = await tonClient.RawGetTransactions(account, rast.LastTransactionId);
            foreach (var item in txs.TransactionsList)
            {
                if (item.InMsg?.Value > 0)
                {
                    logger.LogInformation("TX {Id}: {Value} from {Address}", item.TransactionId.Hash, item.InMsg.Value, item.InMsg.Source.Value);
                }
                else if (item.OutMsgs?.Any() ?? false)
                {
                    logger.LogInformation("TX {Id}: {Value} to {Address}", item.TransactionId.Hash, item.OutMsgs[0].Value, item.OutMsgs[0].Destination.Value);
                }
            }
        }
    }
}
