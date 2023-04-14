using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public static class Program
    {
        // You need mnemonic and address for actual account with some coins to test sending.
        // Double check that you are using testnet!!!
        public const string TestnetAccountToSendFromAddress = "EQAkEWzRLi1sw9AlaGDDzPvk2_F20hjpTjlvsjQqYawVmdT0";
        public static readonly string[] TestnetAccountToSendFromMnemonic = new[]
        {
            "word1", "word2", "word3", "word4", "word5", "word6", "word7", "word8", "word9", "word10", "word11", "word12",
            "word13", "word14", "word15", "word16", "word17", "word18", "word19", "word20", "word21", "word22", "word23", "word24",
        };

        private const bool useMainnet = true; // also replace tonlibjson.dll !

        private const string DirectoryForKeys = "D:/Temp/keys";

        public static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.UseConsoleLifetime();
            builder.ConfigureServices((context, services) =>
            {
                services.Configure<TonOptions>(o =>
                {
                    o.UseMainnet = useMainnet; // also replace tonlibjson.dll !
                    o.LogTextLimit = 500; // Set to 0 to see full requests/responses
                    o.VerbosityLevel = 0;
                    o.Options.KeystoreType = new KeyStoreTypeDirectory(DirectoryForKeys);
                });
                services.AddSingleton<ITonClient, TonClient>();
                services.AddTransient<BalanceAndTransactions>();
                services.AddTransient<KeysAndMnemonics>();
                services.AddTransient<SendTon>();
                services.AddTransient<Extensibility>();
                services.AddTransient<Domains>();
                services.AddTransient<SmartContracts>();
                services.AddTransient<BocAndCells>();
            });

            /// Add types from current assembly (see <see cref="Extensibility"/> class for more info).
            TonClient.RegisterAssembly(typeof(Program).Assembly);

            var app = builder.Build();

            await app.Services.GetRequiredService<BalanceAndTransactions>().Run();
            await app.Services.GetRequiredService<KeysAndMnemonics>().Run();
            await app.Services.GetRequiredService<SendTon>().Run();
            await app.Services.GetRequiredService<Extensibility>().Run();

            if (useMainnet)
            {
                await app.Services.GetRequiredService<Domains>().Run();
            }

            await app.Services.GetRequiredService<SmartContracts>().Run(useMainnet);

            if (useMainnet)
            {
                await app.Services.GetRequiredService<BocAndCells>().Run();
            }

            // Loggers need some time to flush data to screen/console.
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}