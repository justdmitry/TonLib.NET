using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("importKey local_password:secureBytes mnemonic_password:secureBytes exported_key:exportedKey = Key")]
    public class ImportKey : RequestBase<Key>
    {
        public ImportKey(ExportedKey exportedKey)
        {
            ExportedKey = exportedKey ?? throw new ArgumentNullException(nameof(exportedKey));
        }

        public ExportedKey ExportedKey { get; set; }

        public string? LocalPassword { get; set; }

        public string? MnemonicPassword { get; set; }
    }
}
