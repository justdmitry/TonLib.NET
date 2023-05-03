using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TonLibDotNet.Samples;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public static class Program
    {
        private const bool useMainnet = true;

        private const string DirectoryForKeys = "D:/Temp/keys";

        public static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.UseConsoleLifetime();
            builder.ConfigureServices((context, services) =>
            {
                services.Configure<TonOptions>(o =>
                {
                    o.UseMainnet = useMainnet;
                    o.LogTextLimit = 500; // Set to 0 to see full requests/responses
                    o.VerbosityLevel = 0;
                    o.Options.KeystoreType = new KeyStoreTypeDirectory(DirectoryForKeys);
                });
                services.AddSingleton<ITonClient, TonClient>();

                services.AddTransient<GeneralNetworkInfo>();
                services.AddTransient<KeysAndMnemonics>();
                services.AddTransient<AccountBalanceAndTransactions>();
                services.AddTransient<LibraryExtensibility>();
                services.AddTransient<SendTon>();
                services.AddTransient<ResolveDomains>();
                services.AddTransient<ReadInfoFromSmartContracts>();
                services.AddTransient<BocAndCells>();
            });

            /// Add types from current assembly (see <see cref="LibraryExtensibility"/> class for more info).
            TonClient.RegisterAssembly(typeof(Program).Assembly);

            var app = builder.Build();

            await app.Services.GetRequiredService<ITonClient>().InitIfNeeded();

            // Feel free to comment unneeded calls below
            await app.Services.GetRequiredService<GeneralNetworkInfo>().Run();
            await app.Services.GetRequiredService<KeysAndMnemonics>().Run();
            await app.Services.GetRequiredService<AccountBalanceAndTransactions>().Run();
            await app.Services.GetRequiredService<LibraryExtensibility>().Run();
            await app.Services.GetRequiredService<SendTon>().Run(useMainnet);
            await app.Services.GetRequiredService<ResolveDomains>().Run(useMainnet);
            await app.Services.GetRequiredService<ReadInfoFromSmartContracts>().Run(useMainnet);
            await app.Services.GetRequiredService<BocAndCells>().Run(useMainnet);

            // Loggers need some time to flush data to screen/console.
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}