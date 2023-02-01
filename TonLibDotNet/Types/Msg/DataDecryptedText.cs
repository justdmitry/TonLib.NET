namespace TonLibDotNet.Types.Msg
{
    [TLSchema("msg.dataDecryptedText text:bytes = msg.Data")]
    public class DataDecryptedText : Data
    {
        public string Text { get; set; } = string.Empty;
    }
}
