using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("exportUnencryptedKey input_key:InputKey = ExportedUnencryptedKey")]
    public class ExportUnencryptedKey : RequestBase<ExportedUnencryptedKey>
    {
        public ExportUnencryptedKey(InputKey inputKey)
        {
            InputKey = inputKey ?? throw new ArgumentNullException(nameof(inputKey));
        }

        public InputKey InputKey { get; set; }
    }
}
