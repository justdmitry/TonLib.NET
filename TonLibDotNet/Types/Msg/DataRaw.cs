namespace TonLibDotNet.Types.Msg
{
    [TLSchema("msg.dataRaw body:bytes init_state:bytes = msg.Data")]
    public class DataRaw : Data
    {
        public DataRaw(string body, string initState)
        {
            Body = body;
            InitState = initState;
        }

        public string Body { get; set; }

        public string InitState { get; set; }
    }
}
