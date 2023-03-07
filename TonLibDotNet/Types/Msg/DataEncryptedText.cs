namespace TonLibDotNet.Types.Msg
{
    [TLSchema("msg.dataEncryptedText text:bytes = msg.Data")]
    public class DataEncryptedText : Data
    {
        public DataEncryptedText(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}
