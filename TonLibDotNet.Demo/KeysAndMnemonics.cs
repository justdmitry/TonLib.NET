using Microsoft.Extensions.Logging;

namespace TonLibDotNet
{
    public class KeysAndMnemonics
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public KeysAndMnemonics(ITonClient tonClient, ILogger<KeysAndMnemonics> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run()
        {
            await tonClient.InitIfNeeded();

            // Get hint for user typing mnemonic words in your app
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
    }
}