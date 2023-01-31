using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TonLibDotNet.Requests;

namespace TonLibDotNet
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.UseConsoleLifetime();
            builder.ConfigureServices((context, services) =>
            {
                services.Configure<TonOptions>(o =>
                {
                    o.UseMainnet = true;
                    o.LogTextLimit = 500;
                });
                services.AddSingleton<ITonClient, TonClient>();
            });

            var app = builder.Build();

            var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(Program));

            var tonClient = app.Services.GetRequiredService<ITonClient>();

            await tonClient.InitIfNeeded();

            var lsi = tonClient.Execute(new LiteServerGetInfo());
            logger.LogInformation("Server time: {Now}", lsi.Now);

            var mi = tonClient.Execute(new GetMasterchainInfo());
            logger.LogInformation("Last block: shard = {Shard}, seqno = {Seqno}", mi.Last.Shard, mi.Last.Seqno);

            // Loggers need some time to flush data to screen/console.
            await Task.Delay(2000);
        }
    }
}