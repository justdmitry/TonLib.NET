using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            ////builder.ConfigureLogging(o => o.AddSystemdConsole());
            builder.UseConsoleLifetime();
            builder.ConfigureServices((context, services) =>
            {
                services.Configure<TonOptions>(o =>
                {
                    o.UseMainnet = false;
                    o.LogTextLimit = 0;
                    o.VerbosityLevel = 0;
                    o.Options.KeystoreType = new KeyStoreTypeDirectory("D:/Temp/keys");
                });
                services.AddSingleton<ITonClient, TonClient>();
            });

            var app = builder.Build();

            var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(Program));

            var tonClient = app.Services.GetRequiredService<ITonClient>();

            await tonClient.InitIfNeeded();

            var lsi = await tonClient.LiteServerGetInfo();
            logger.LogInformation("Server time: {Now}", lsi.Now);

            var mi = await tonClient.GetMasterchainInfo();
            logger.LogInformation("Last block: shard = {Shard}, seqno = {Seqno}", mi.Last.Shard, mi.Last.Seqno);

            var account = "EQCJTkhd1W2wztkVNp_dsKBpv2SIoUWoIyzI7mQrbSrj_NSh";

            var ast = await tonClient.GetAccountState(account); // TON Diamonds
            logger.LogInformation("Acc info: balance = {Value}", ast.Balance);

            var rast = await tonClient.RawGetAccountState(account); // TON Diamonds
            logger.LogInformation("Acc info: balance = {Value}", rast.Balance);

            var txs = await tonClient.RawGetTransactions(account, rast.LastTransactionId);
            foreach(var item in txs.TransactionsList)
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

            // Loggers need some time to flush data to screen/console.
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}