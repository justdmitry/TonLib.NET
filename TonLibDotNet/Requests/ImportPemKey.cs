using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("importPemKey local_password:secureBytes key_password:secureBytes exported_key:exportedPemKey = Key")]
    public class ImportPemKey : RequestBase<Key>
    {
        public ImportPemKey(ExportedPemKey exportedKey)
        {
            ExportedKey = exportedKey ?? throw new ArgumentNullException(nameof(exportedKey));
        }

        public ExportedPemKey ExportedKey { get; set; }

        public string? LocalPassword { get; set; }

        public string? KeyPassword { get; set; }
    }
}
