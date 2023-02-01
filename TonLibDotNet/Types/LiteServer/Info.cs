namespace TonLibDotNet.Types.LiteServer
{
    [TLSchema("liteServer.info now:int53 version:int32 capabilities:int64 = liteServer.Info")]
    public class Info : TypeBase
    {
        public DateTimeOffset Now { get; set; }

        public int Version { get; set; }

        public long Capabilities { get; set; }
    }
}
