namespace TonLibDotNet.Types
{
    [TLSchema("inputKeyRegular key:key local_password:secureBytes = InputKey")]
    public class InputKeyRegular : InputKey
    {
        public InputKeyRegular(Key key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public Key Key { get; set; }

        public string? LocalPassword { get; set; }
    }
}
