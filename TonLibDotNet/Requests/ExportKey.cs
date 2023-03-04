using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("exportKey input_key:InputKey = ExportedKey")]
    public class ExportKey : RequestBase<ExportedKey>
    {
        public ExportKey(InputKey inputKey)
        {
            InputKey = inputKey ?? throw new ArgumentNullException(nameof(inputKey));
        }

        public InputKey InputKey { get; set; }
    }
}
