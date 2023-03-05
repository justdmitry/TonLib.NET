using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("importUnencryptedKey local_password:secureBytes exported_unencrypted_key:exportedUnencryptedKey = Key")]
    public class ImportUnencryptedKey : RequestBase<Key>
    {
        public ImportUnencryptedKey(ExportedUnencryptedKey exportedUnencryptedKey)
        {
            ExportedUnencryptedKey = exportedUnencryptedKey ?? throw new ArgumentNullException(nameof(exportedUnencryptedKey));
        }

        public ExportedUnencryptedKey ExportedUnencryptedKey { get; set; }

        public string? LocalPassword { get; set; }
    }
}
