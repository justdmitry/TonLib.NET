namespace TonLibDotNet.Types.Msg
{
    [TLSchema("msg.dataRaw body:bytes init_state:bytes = msg.Data")]
    public class DataRaw : Data
    {
        public string Body { get; set; } = string.Empty;

        public string InitState { get; set; } = string.Empty;
    }
}
