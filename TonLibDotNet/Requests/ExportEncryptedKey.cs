using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("exportEncryptedKey input_key:InputKey key_password:secureBytes = ExportedEncryptedKey")]
    public class ExportEncryptedKey : RequestBase<ExportedEncryptedKey>
    {
        public ExportEncryptedKey(InputKey inputKey)
        {
            InputKey = inputKey ?? throw new ArgumentNullException(nameof(inputKey));
        }

        public InputKey InputKey { get; set; }

        public string? KeyPassword { get; set; }
    }
}
