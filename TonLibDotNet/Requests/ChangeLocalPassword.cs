using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("changeLocalPassword input_key:InputKey new_local_password:secureBytes = Key")]
    public class ChangeLocalPassword : RequestBase<Key>
    {
        public ChangeLocalPassword(InputKey inputKey, string? newLocalPassword)
        {
            InputKey = inputKey ?? throw new ArgumentNullException(nameof(inputKey));
            NewLocalPassword = newLocalPassword;
        }

        public InputKey InputKey { get; set; }

        public string? NewLocalPassword { get; set; }
    }
}
