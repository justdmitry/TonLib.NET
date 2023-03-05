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
                    o.UseMainnet = true;
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

            var account = "EQCJTkhd1W2wztkVNp_dsKBpv2SIoUWoIyzI7mQrbSrj_NSh"; // TON Diamonds

            var uaa = await tonClient.UnpackAccountAddress(account);
            var paa = await tonClient.PackAccountAddress(uaa);

            var ast = await tonClient.GetAccountState(account);
            logger.LogInformation("Acc info: balance = {Value}", ast.Balance);

            var rast = await tonClient.RawGetAccountState(account);
            logger.LogInformation("Acc info: balance = {Value}", rast.Balance);

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

            var hints = await tonClient.GetBip39Hints("zo");

            await RunKeyDemo(tonClient);

            // Loggers need some time to flush data to screen/console.
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        private static async Task RunKeyDemo(ITonClient tonClient)
        {
            // some "random" bytes
            var localPass = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 });
            var mnemonicPass = Convert.ToBase64String(new byte[] { 19, 42, 148 });
            var randomExtra = Convert.ToBase64String(new byte[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 });
            var keyPass = Convert.ToBase64String(new byte[] { 21, 3, 7, 11 });

            var key = await tonClient.CreateNewKey(localPass, mnemonicPass, randomExtra);
            var ek = await tonClient.ExportKey(key, localPass);
            var epk = await tonClient.ExportPemKey(key, localPass, keyPass);
            var eek = await tonClient.ExportEncryptedKey(key, localPass, keyPass);
            var euk = await tonClient.ExportUnencryptedKey(key, localPass);

            //// does not work, see https://github.com/ton-blockchain/ton/issues/202
            //// key = await tonClient.ChangeLocalPassword(key, localPass, Convert.ToBase64String(new byte[] { 7, 6, 5 }));

            await tonClient.DeleteKey(key);

            var key2 = await tonClient.ImportKey(ek, localPass, mnemonicPass);
            await tonClient.DeleteKey(key2);

            var key3 = await tonClient.ImportPemKey(epk, localPass, keyPass);
            await tonClient.DeleteKey(key3);

            var key4 = await tonClient.ImportEncryptedKey(eek, localPass, keyPass);
            await tonClient.DeleteKey(key4);

            var key5 = await tonClient.ImportUnencryptedKey(euk, localPass);
            await tonClient.DeleteKey(key5);

            await tonClient.DeleteAllKeys();
        }
    }
}