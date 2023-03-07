namespace TonLibDotNet.Types.Msg
{
    [TLSchema("msg.dataText text:bytes = msg.Data")]
    public class DataText : Data
    {
        public DataText(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}
