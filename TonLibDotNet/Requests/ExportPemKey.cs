using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("exportPemKey input_key:InputKey key_password:secureBytes = ExportedPemKey")]
    public class ExportPemKey : RequestBase<ExportedPemKey>
    {
        public ExportPemKey(InputKey inputKey)
        {
            InputKey = inputKey ?? throw new ArgumentNullException(nameof(inputKey));
        }

        public InputKey InputKey { get; set; }

        public string? KeyPassword { get; set; }
    }
}
