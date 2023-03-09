using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TonLibDotNet.Types;
using TonLibDotNet.Types.Wallet;

namespace TonLibDotNet
{
    public static class Program
    {
        private const string DirectoryForKeys = "D:/Temp/keys";

        // You need mnemonic and address for actual account with some coins to test sending.
        // Double check that you are using testnet!!!
        private const string TestnetAccountToSendFromAddress = "EQAkEWzRLi1sw9AlaGDDzPvk2_F20hjpTjlvsjQqYawVmdT0";
        private static readonly string[] TestnetAccountToSendFromMnemonic = new[]
        {
            "word1", "word2", "word3", "word4", "word5", "word6", "word7", "word8", "word9", "word10", "word11", "word12",
            "word13", "word14", "word15", "word16", "word17", "word18", "word19", "word20", "word21", "word22", "word23", "word24",
        };

        public static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.UseConsoleLifetime();
            builder.ConfigureServices((context, services) =>
            {
                services.Configure<TonOptions>(o =>
                {
                    o.UseMainnet = false; // also replace tonlibjson.dll !
                    o.LogTextLimit = 500; // Set to 0 to see full requests/responses
                    o.VerbosityLevel = 0;
                    o.Options.KeystoreType = new KeyStoreTypeDirectory(DirectoryForKeys);
                });
                services.AddSingleton<ITonClient, TonClient>();
            });

            /// Add types from current assembly (see <see cref="ExtensibilityDemoRequest"/> class and <see cref="RunExtensibilityDemo(ITonClient)" below />).
            TonClient.RegisterAssembly(typeof(Program).Assembly);

            var app = builder.Build();

            var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(Program));

            var tonClient = app.Services.GetRequiredService<ITonClient>();

            await tonClient.InitIfNeeded();

            var lsi = await tonClient.LiteServerGetInfo();
            logger.LogInformation("Server time: {Now}", lsi.Now);

            var mi = await tonClient.GetMasterchainInfo();
            logger.LogInformation("Last block: shard = {Shard}, seqno = {Seqno}", mi.Last.Shard, mi.Last.Seqno);

            await RunAssortDemo(tonClient, logger);

            await RunKeyDemo(tonClient);

            if (TestnetAccountToSendFromMnemonic[0] != "word1")
            {
                await RunSendDemo(tonClient, logger, TestnetAccountToSendFromAddress, TestnetAccountToSendFromMnemonic);
            }

            await RunExtensibilityDemo(tonClient, logger);

            // Loggers need some time to flush data to screen/console.
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        private static async Task RunAssortDemo(ITonClient tonClient, ILogger logger)
        {
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
        }

        private static async Task RunKeyDemo(ITonClient tonClient)
        {
            var hints = await tonClient.GetBip39Hints("zo");

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

        private static async Task RunSendDemo(ITonClient tonClient, ILogger logger, string validAddress, string[] mnemonic)
        {
            /*
             * See https://ton.org/docs/develop/dapps/asset-processing/#deploying-wallet
             * and https://ton.org/docs/develop/dapps/asset-processing/#sending-payments
             */

            // Step 1: Import key and find your address

            // Docs says you should use value from network config.
            var walletId = tonClient.OptionsInfo.ConfigInfo.DefaultWalletId;

            // Surprise! Even for testnet, wallet.ton.org uses mainnet value :(
            walletId = 698983191;

            var inputKey = await tonClient.ImportKey(new ExportedKey(mnemonic.ToList()));
            var initialAccountState = new V3InitialAccountState() { PublicKey = inputKey.PublicKey, WalletId = walletId };
            var address = await tonClient.GetAccountAddress(initialAccountState, 0, 0);
            logger.LogDebug("Verifying addresses: expected '{Valid}', got '{Actual}'", validAddress, address.Value);
            if (validAddress != address.Value)
            {
                logger.LogError("Address mismatch. Aborting.");
                return;
            }

            // Step 2: Build message and action
            var msg = new Types.Msg.Message(new AccountAddress(validAddress))
            {
                Data = new Types.Msg.DataText(tonClient.EncodeStringAsBase64("Sent using https://github.com/justdmitry/TonLib.NET")),
                Amount = tonClient.ConvertToNanoTon(0.01M),
                SendMode = 1,
            };

            var action = new Types.ActionMsg(new List<Types.Msg.Message>() { msg }) { AllowSendToUninited = true };

            // Step 3: create query and send it
            var query = await tonClient.CreateQuery(new InputKeyRegular(inputKey), address, action, TimeSpan.FromMinutes(1), initialAccountState: initialAccountState);

            // wanna know fees before sending?
            var fees = await tonClient.QueryEstimateFees(query.Id);

            // Send it to network. You dont have TX id or something in respnse - just poll getTransactions() for your account and wait for new TX.
            _ = await tonClient.QuerySend(query.Id);
        }

        private static async Task RunExtensibilityDemo(ITonClient tonClient, ILogger logger)
        {
            // This will fail because TonLib does not have this method.
            // This is just a demo how you can add new types/requests without waiting for new package release.
            // To make this happen in your app - call TonClient.RegisterAssembly() early
            try
            {
                await tonClient.Execute(new ExtensibilityDemoRequest() { Continent = "Antarctica" });
            }
            catch (TonClientException ex)
            {
                logger.LogWarning(ex, "Exception ignored");
                // Ignore
            }
        }
    }
}