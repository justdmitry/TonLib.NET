using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TonLibDotNet.Samples;
using TonLibDotNet.Samples.Recipes;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public static class Program
    {
        // You will need actual mnemonic and address with some coins to run tests like SendTon, SendJetton etc.
        // These tests have safeguards and will not run on mainnet.
        // But anyway, double check that you are using testnet and what tests are uncommented before putting actual seed phrase here!!!
        public const string TestAddress = "EQAkEWzRLi1sw9AlaGDDzPvk2_F20hjpTjlvsjQqYawVmdT0";
        public const string TestMnemonic = ""; // put space-delimited mnemonic words here

        // Some tests need mainnet (e.g. domains), some will run only in testnet (e.g. sending coins).
        public const bool UseMainnet = true;

        private const string DirectoryForKeys = "D:/Temp/keys";

        public static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.UseConsoleLifetime();
            builder.ConfigureServices((context, services) =>
            {
                services.Configure<TonOptions>(o =>
                {
                    o.UseMainnet = UseMainnet;
                    o.LogTextLimit = 500; // Set to 0 to see full requests/responses
                    o.VerbosityLevel = 0;
                    o.Options.KeystoreType = new KeyStoreTypeDirectory(DirectoryForKeys);
                });

                services.AddHostedService<SamplesRunner>();

                services.AddSingleton<ITonClient, TonClient>();

                services.AddTransient<ISample, GeneralNetworkInfo>();
                services.AddTransient<ISample, KeysAndMnemonics>();
                services.AddTransient<ISample, AccountBalanceAndTransactions>();
                services.AddTransient<ISample, LibraryExtensibility>();
                services.AddTransient<ISample, SendTon>();
                services.AddTransient<ISample, ResolveDomains>();
                services.AddTransient<ISample, ReadInfoFromSmartContracts>();
                services.AddTransient<ISample, BocAndCells>();
                services.AddTransient<ISample, DomainAuctionInfo>();
                services.AddTransient<ISample, RootDnsGetAllInfo>();
                services.AddTransient<ISample, TelemintGetAllInfo>();
                services.AddTransient<ISample, Jettons>();
            });

            /// Add types from current assembly (see <see cref="LibraryExtensibility"/> class for more info).
            TonClient.RegisterAssembly(typeof(Program).Assembly);

            var app = builder.Build();
            await app.RunAsync();
        }
    }
}