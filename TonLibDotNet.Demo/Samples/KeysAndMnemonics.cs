using Microsoft.Extensions.Logging;

namespace TonLibDotNet.Samples
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

            // Building wallet and want to help your users to enter mnemonic? Show them hints while they type.
            var hints = await tonClient.GetBip39Hints("zo");
            logger.LogInformation("Mnemonic words starting from '{Letters}' are: {List}", "zo", string.Join(", ", hints.Words));

            // some "random" bytes
            var localPass = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 });
            var mnemonicPass = Convert.ToBase64String(new byte[] { 19, 42, 148 });
            var randomExtra = Convert.ToBase64String(new byte[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 });
            var keyPass = Convert.ToBase64String(new byte[] { 21, 3, 7, 11 });

            // create new key
            var key = await tonClient.CreateNewKey(localPass, mnemonicPass, randomExtra);
            logger.LogInformation("New key: public = {PublicKey}, secret = {Secret}", key.PublicKey, key.Secret);

            var ek = await tonClient.ExportKey(key, localPass);
            logger.LogInformation("Mnemonic for this key is: {Words}", string.Join(" ", ek.WordList));

            var epk = await tonClient.ExportPemKey(key, localPass, keyPass);
            logger.LogInformation("Same key in PEM with password:\r\n{PEM}", epk.Pem);

            var eek = await tonClient.ExportEncryptedKey(key, localPass, keyPass);
            logger.LogInformation("Same key exported with password: {Value}", eek.Data);

            var euk = await tonClient.ExportUnencryptedKey(key, localPass);
            logger.LogInformation("Same key in unencrypted form: {Value}", euk.Data);

            //// does not work, see https://github.com/ton-blockchain/ton/issues/202
            //// key = await tonClient.ChangeLocalPassword(key, localPass, Convert.ToBase64String(new byte[] { 7, 6, 5 }));

            // Delete key from tonlib local storage if you don't need them anymore.
            await tonClient.DeleteKey(key);

            // You can import keys back
            var key2 = await tonClient.ImportKey(ek, localPass, mnemonicPass);
            await tonClient.DeleteKey(key2);

            var key3 = await tonClient.ImportPemKey(epk, localPass, keyPass);
            await tonClient.DeleteKey(key3);

            var key4 = await tonClient.ImportEncryptedKey(eek, localPass, keyPass);
            await tonClient.DeleteKey(key4);

            var key5 = await tonClient.ImportUnencryptedKey(euk, localPass);
            await tonClient.DeleteKey(key5);

            // Delete all keys from tonlib local storage.
            await tonClient.DeleteAllKeys();
        }
    }
}