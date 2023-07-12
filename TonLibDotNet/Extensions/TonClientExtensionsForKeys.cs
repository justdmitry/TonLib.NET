using TonLibDotNet.Requests;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public static class TonClientExtensionsForKeys
    {
        /// <summary>
        /// Creates new keypair.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="localPassword">Local password (base64 bytes).</param>
        /// <param name="mnemonicPassword">Mnemonic password (base64 bytes).</param>
        /// <param name="randomExtraSeed">Extra seed (base64 bytes).</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L4273" />
        public static Task<Key> CreateNewKey(this ITonClient client, string? localPassword = null, string? mnemonicPassword = null, string? randomExtraSeed = null)
        {
            return client.Execute(new CreateNewKey() { LocalPassword = localPassword, MnemonicPassword = mnemonicPassword, RandomExtraSeed = randomExtraSeed });
        }

        /// <summary>
        /// Delete all stored keys.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L4310" />
        public static Task<Ok> DeleteAllKeys(this ITonClient client)
        {
            return client.Execute(new DeleteAllKeys());
        }

        /// <summary>
        /// Delete stored key.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="key">Key to delete.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L4310" />
        public static Task<Ok> DeleteKey(this ITonClient client, Key key)
        {
            ArgumentNullException.ThrowIfNull(key);

            return client.Execute(new DeleteKey(key));
        }

        /// <summary>
        /// Returns mnemonic (24 words) that corresponds to provided key.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="key">Key to get mnemonic for.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L4285" />
        public static Task<ExportedKey> ExportKey(this ITonClient client, InputKey key)
        {
            ArgumentNullException.ThrowIfNull(key);

            return client.Execute(new ExportKey(key));
        }

        /// <inheritdoc cref="ExportKey(ITonClient, InputKey)"/>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="key">Key to export.</param>
        /// <param name="localPassword">Local key password (base64 bytes).</param>
        public static Task<ExportedKey> ExportKey(this ITonClient client, Key key, string? localPassword = null)
        {
            ArgumentNullException.ThrowIfNull(key);

            return ExportKey(client, new InputKeyRegular(key) { LocalPassword = localPassword });
        }

        /// <summary>
        /// Export key to PEM format.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="key">Key to export.</param>
        /// <param name="keyPassword">Password to protect exported key (base64 bytes).</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L4329" />
        public static Task<ExportedPemKey> ExportPemKey(this ITonClient client, InputKey key, string? keyPassword = null)
        {
            ArgumentNullException.ThrowIfNull(key);

            return client.Execute(new ExportPemKey(key) { KeyPassword = keyPassword });
        }

        /// <inheritdoc cref="ExportPemKey(ITonClient, InputKey, string?)"/>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="key">Key to export.</param>
        /// <param name="localPassword">Local key password (base64 bytes).</param>
        /// <param name="keyPassword">Password to protect exported key (base64 bytes).</param>
        public static Task<ExportedPemKey> ExportPemKey(this ITonClient client, Key key, string? localPassword = null, string? keyPassword = null)
        {
            ArgumentNullException.ThrowIfNull(key);

            return ExportPemKey(client, new InputKeyRegular(key) { LocalPassword = localPassword }, keyPassword);
        }

        /// <summary>
        /// Export key with encryption.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="key">Key to export.</param>
        /// <param name="keyPassword">Password to protect exported key (base64 bytes).</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L4352" />
        public static Task<ExportedEncryptedKey> ExportEncryptedKey(this ITonClient client, InputKey key, string? keyPassword = null)
        {
            ArgumentNullException.ThrowIfNull(key);

            return client.Execute(new ExportEncryptedKey(key) { KeyPassword = keyPassword });
        }

        /// <inheritdoc cref="ExportEncryptedKey(ITonClient, InputKey, string?)"/>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="key">Key to export.</param>
        /// <param name="localPassword">Local key password (base64 bytes).</param>
        /// <param name="keyPassword">Password to protect exported key (base64 bytes).</param>
        public static Task<ExportedEncryptedKey> ExportEncryptedKey(this ITonClient client, Key key, string? localPassword = null, string? keyPassword = null)
        {
            ArgumentNullException.ThrowIfNull(key);

            return ExportEncryptedKey(client, new InputKeyRegular(key) { LocalPassword = localPassword }, keyPassword);
        }

        /// <summary>
        /// Export key without encryption.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="key">Key to export.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L4375" />
        public static Task<ExportedUnencryptedKey> ExportUnencryptedKey(this ITonClient client, InputKey key)
        {
            ArgumentNullException.ThrowIfNull(key);

            return client.Execute(new ExportUnencryptedKey(key));
        }

        /// <inheritdoc cref="ExportUnencryptedKey(ITonClient, InputKey)"/>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="key">Key to export.</param>
        /// <param name="localPassword">Local key password (base64 bytes).</param>
        public static Task<ExportedUnencryptedKey> ExportUnencryptedKey(this ITonClient client, Key key, string? localPassword = null)
        {
            ArgumentNullException.ThrowIfNull(key);

            return ExportUnencryptedKey(client, new InputKeyRegular(key) { LocalPassword = localPassword });
        }

        /// <summary>
        /// Import key using mnemonic words.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="key">Key to import.</param>
        /// <param name="localPassword">Local key password (base64 bytes).</param>
        /// <param name="mnemonicPassword">Mnemonic password (base64 bytes).</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L4317" />
        public static Task<Key> ImportKey(this ITonClient client, ExportedKey key, string? localPassword = null, string? mnemonicPassword = null)
        {
            ArgumentNullException.ThrowIfNull(key);

            return client.Execute(new ImportKey(key) { LocalPassword = localPassword, MnemonicPassword = mnemonicPassword });
        }

        /// <summary>
        /// Import key from PEM.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="key">Key to import.</param>
        /// <param name="localPassword">Local key password (base64 bytes).</param>
        /// <param name="keyPassword">Exported key protection password (base64 bytes).</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L4340" />
        public static Task<Key> ImportPemKey(this ITonClient client, ExportedPemKey key, string? localPassword = null, string? keyPassword = null)
        {
            ArgumentNullException.ThrowIfNull(key);

            return client.Execute(new ImportPemKey(key) { LocalPassword = localPassword, KeyPassword = keyPassword });
        }

        /// <summary>
        /// Import key from encrypted form.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="key">Key to import.</param>
        /// <param name="localPassword">Local key password (base64 bytes).</param>
        /// <param name="keyPassword">Exported key protection password (base64 bytes).</param>
        /// <param name="mnemonicPassword">Mnemonic password (base64 bytes).</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L4363" />
        public static Task<Key> ImportEncryptedKey(this ITonClient client, ExportedEncryptedKey key, string? localPassword = null, string? keyPassword = null, string? mnemonicPassword = null)
        {
            ArgumentNullException.ThrowIfNull(key);

            return client.Execute(new ImportEncryptedKey(key) { LocalPassword = localPassword, KeyPassword = keyPassword, MnemonicPassword = mnemonicPassword });
        }

        /// <summary>
        /// Import key from unencrypted form.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="key">Key to import.</param>
        /// <param name="localPassword">Local key password (base64 bytes).</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L4385" />
        public static Task<Key> ImportUnencryptedKey(this ITonClient client, ExportedUnencryptedKey key, string? localPassword = null)
        {
            ArgumentNullException.ThrowIfNull(key);

            return client.Execute(new ImportUnencryptedKey(key) { LocalPassword = localPassword });
        }

        /// <summary>
        /// Chnages local password for already existing key.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="key">Key to change password for.</param>
        /// <param name="newLocalPassword">New password (base64 bytes).</param>
        /// <returns></returns>
        /// <seealso href="https://github.com/ton-blockchain/ton/issues/202">Method does not work. See github issue 202.</seealso>
        public static Task<Key> ChangeLocalPassword(this ITonClient client, InputKey key, string? newLocalPassword = null)
        {
            ArgumentNullException.ThrowIfNull(key);

            return client.Execute(new ChangeLocalPassword(key, newLocalPassword));
        }

        /// <inheritdoc cref="ChangeLocalPassword(ITonClient, InputKey, string?)"/>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="key">Key to change password for.</param>
        /// <param name="localPassword">Local key password (base64 bytes).</param>
        /// <param name="newLocalPassword">New local key password (base64 bytes).</param>
        public static Task<Key> ChangeLocalPassword(this ITonClient client, Key key, string? localPassword = null, string? newLocalPassword = null)
        {
            ArgumentNullException.ThrowIfNull(key);

            return ChangeLocalPassword(client, new InputKeyRegular(key) { LocalPassword = localPassword }, newLocalPassword);
        }
    }
}
