namespace TonLibDotNet.Types.Msg
{
    [TLSchema("msg.dataEncryptedText text:bytes = msg.Data")]
    public class DataEncryptedText : Data
    {
        public string Text { get; set; } = string.Empty;
    }
}
