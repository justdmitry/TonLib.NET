namespace TonLibDotNet.Types.Msg
{
    [TLSchema("msg.dataDecryptedText text:bytes = msg.Data")]
    public class DataDecryptedText : Data
    {
        public DataDecryptedText(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}
