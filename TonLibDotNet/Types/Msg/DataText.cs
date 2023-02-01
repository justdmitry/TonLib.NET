namespace TonLibDotNet.Types.Msg
{
    [TLSchema("msg.dataText text:bytes = msg.Data")]
    public class DataText : Data
    {
        public string Text { get; set; } = string.Empty;
    }
}
