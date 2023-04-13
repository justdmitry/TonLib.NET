using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TonLibDotNet.Cells;
using TonLibDotNet.Types;
using TonLibDotNet.Types.Dns;
using TonLibDotNet.Types.Smc;
using TonLibDotNet.Types.Tvm;
using TonLibDotNet.Types.Wallet;

namespace TonLibDotNet
{
    public static class Program
    {
        private const string DirectoryForKeys = "D:/Temp/keys";

        // You need mnemonic and address for actual account with some coins to test sending.
        // Double check that you are using testnet!!!
        private const bool useMainnet = true; // also replace tonlibjson.dll !
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
                    o.UseMainnet = useMainnet; // also replace tonlibjson.dll !
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

            if (useMainnet)
            {
                await RunDnsDemo(tonClient, logger);
            }

            if (useMainnet)
            {
                await RunMainnetSmcDemo(tonClient, logger);
            }
            else
            {
                await RunTestnetSmcDemo(tonClient);
            }

            if (useMainnet)
            {
                await RunCellsDemo(tonClient, logger);
            }

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

        private static async Task RunDnsDemo(ITonClient tonClient, ILogger logger)
        {
            // Option 1: use non-empty TTL
            var res = await tonClient.DnsResolve("toncenter.ton", ttl: 9);
            var adnl1 = (res.Entries[0].Value as EntryDataAdnlAddress)?.AdnlAddress.Value;

            // Option 2: Use zero TTL and recurse yourself
            res = await tonClient.DnsResolve("ton.", null, null, null);
            // Now we have NFT Collection (EQC3dNlesgVD8YbAazcauIrXBPfiVhMMr5YYk2in0Mtsz0Bz) in 'NextResolver'
            // Let's ask it for next part
            res = await tonClient.DnsResolve("toncenter.", (res.Entries[0].Value as EntryDataNextResolver).Resolver, null, null);
            // Now we have NFT itself (Contract Type = Domain, toncenter.ton, EQAiIsvar4OYBn8BGBf9flfin6tl5poBx4MgJe4CQJYasy51) in 'NextResolver'
            var nftAccountAddress = (res.Entries[0].Value as EntryDataNextResolver).Resolver;
            // Let's ask it about actual ADNL of this domain
            res = await tonClient.DnsResolve(".", nftAccountAddress, null, null);
            // And now we have ADNL address in answer
            var adnl2 = (res.Entries[0].Value as EntryDataAdnlAddress)?.AdnlAddress.Value;

            logger.LogInformation("Results:\r\nADNL1 {Val}\r\nADNL2 {Val}", adnl1, adnl2);

            // Some experiments. Try TTL=1
            res = await tonClient.DnsResolve("toncenter.ton", null, 1);
            // We now have again 'EQAiIsvar4OYBn8BGBf9flfin6tl5poBx4MgJe4CQJYasy51' in 'NextResolver' !
            // You see? TTL is a number of 'recursive iterations' to 'NextResolvers' that is performed by LiteServer (or tonlib?) itself.
            // Checking with TTL=2, we should receive final ADNL record:
            res = await tonClient.DnsResolve("toncenter.ton", null, 2);
            var adnl3 = (res.Entries[0].Value as EntryDataAdnlAddress)?.AdnlAddress.Value; // Yes, we do!

            // Unfortunately, asking for account state of NFT itself returns raw.AccountState, not Dns.AccountState, I don't know why :(
            _ = await tonClient.GetAccountState("EQAiIsvar4OYBn8BGBf9flfin6tl5poBx4MgJe4CQJYasy51");
            // But check RunMainnetSmcDemo() to know how to get owner of this NFT!
        }

        private static async Task RunTestnetSmcDemo(ITonClient tonClient)
        {
            // https://ton-community.github.io/tutorials/02-contract/
            const string adr = "EQAHI1vGuw7d4WG-CtfDrWqEPNtmUuKjKFEFeJmZaqqfWTvW";

            var info = await tonClient.SmcLoad(new AccountAddress(adr));
            _ = await tonClient.SmcGetCode(info.Id);
            _ = await tonClient.SmcGetState(info.Id);
            _ = await tonClient.SmcGetData(info.Id);
            _ = await tonClient.SmcRunGetMethod(info.Id, new MethodIdName("counter"));

            // var libs = await tonClient.Execute(new GetLibraries("1234567890"));

            _ = await tonClient.SmcForget(info.Id);
        }

        private static async Task RunMainnetSmcDemo(ITonClient tonClient, ILogger logger)
        {
            // https://tonapi.io/account/EQDendoireMDFMufOUzkqNpFIay83GnjV2tgGMbA64wA3siV
            const string adr = "EQDendoireMDFMufOUzkqNpFIay83GnjV2tgGMbA64wA3siV"; // tonapi.ton domain NFT

            var info = await tonClient.SmcLoad(adr);

            var rr = await tonClient.SmcRunGetMethod(info.Id, new MethodIdName("get_domain"));
            if (Boc.TryParseFromBase64(((StackEntryCell)rr.Stack[0]).Cell.Bytes, out var boc))
            {
                logger.LogInformation("Domain (expecting 'tonapi'): {Value}", Encoding.ASCII.GetString(boc.RootCells[0].Content));
            }

            rr = await tonClient.SmcRunGetMethod(info.Id, new MethodIdName("get_nft_data"));
            if (Boc.TryParseFromBase64(((StackEntryCell)rr.Stack[3]).Cell.Bytes, out boc))
            {
                var slice = boc.RootCells[0].BeginRead();
                var ads = slice.LoadAddressIntStd();
                slice.EndRead();

                logger.LogInformation("Owner (expecting 'EQCNdbNc28ZrcE3AKGDqK18-NFbcSzhTGaRPeEqnMIJiQsl_'): {Value}", ads);
            }

            _ = await tonClient.SmcForget(info.Id);
        }

        private static async Task RunCellsDemo(ITonClient tonClient, ILogger logger)
        {
            // If you know storage layout for some smartcontract,
            //    Than you can parse it "manually", without calling get-methods

            // https://tonapi.io/account/EQDendoireMDFMufOUzkqNpFIay83GnjV2tgGMbA64wA3siV
            const string adr = "EQDendoireMDFMufOUzkqNpFIay83GnjV2tgGMbA64wA3siV"; // tonapi.ton domain NFT

            var info = await tonClient.SmcLoad(adr);
            var data = await tonClient.SmcGetData(info.Id);
            var boc = Boc.ParseFromBase64(data.Bytes);
            logger.LogInformation("BOC:\r\n{Text}", boc.DumpCells());

            var domain = Encoding.ASCII.GetString(boc.RootCells[0].Refs[1].Content);
            logger.LogInformation("Domain (expecting 'tonapi'): {Value}", domain);

            // Storage
            //
            // uint256 index
            // MsgAddressInt collection_address
            // MsgAddressInt owner_address
            // cell content
            // cell domain -e.g contains "alice"(without ending \0) for "alice.ton" domain
            // cell auction - auction info
            // int last_fill_up_time
            var rootSlice = boc.RootCells[0].BeginRead();
            rootSlice.SkipBits(256);
            var collectionAddress = rootSlice.LoadAddressIntStd();
            logger.LogInformation("Collection (expecting 'EQC3dNlesgVD8YbAazcauIrXBPfiVhMMr5YYk2in0Mtsz0Bz'): {Value}", collectionAddress);

            var ownerAddress = rootSlice.LoadAddressIntStd();
            logger.LogInformation("Owner (expecting 'EQCNdbNc28ZrcE3AKGDqK18-NFbcSzhTGaRPeEqnMIJiQsl_'): {Value}", ownerAddress);
        }
    }
}