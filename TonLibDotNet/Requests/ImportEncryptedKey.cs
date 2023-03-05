using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("importEncryptedKey local_password:secureBytes key_password:secureBytes exported_encrypted_key:exportedEncryptedKey = Key")]
    public class ImportEncryptedKey : RequestBase<Key>
    {
        public ImportEncryptedKey(ExportedEncryptedKey exportedEncryptedKey)
        {
            ExportedEncryptedKey = exportedEncryptedKey ?? throw new ArgumentNullException(nameof(exportedEncryptedKey));
        }

        public ExportedEncryptedKey ExportedEncryptedKey { get; set; }

        public string? LocalPassword { get; set; }

        public string? KeyPassword { get; set; }

        public string? MnemonicPassword { get; set; }
    }
}
