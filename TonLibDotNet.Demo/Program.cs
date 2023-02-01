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
                    o.LogTextLimit = 500;
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

            var bi = await tonClient.GetAccountState("EQCJTkhd1W2wztkVNp_dsKBpv2SIoUWoIyzI7mQrbSrj_NSh"); // TON Diamonds
            logger.LogInformation("Acc info: balance = {Value}", bi.Balance);

            // Loggers need some time to flush data to screen/console.
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}