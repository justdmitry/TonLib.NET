namespace TonLibDotNet.Types.LiteServer
{
    /// <remarks>
    /// TL Schema:
    /// <code>liteServer.info now:int53 version:int32 capabilities:int64 = liteServer.Info;</code>
    /// </remarks>
    public class Info : TypeBase
    {
        public Info()
        {
            TypeName = "liteServer.info";
        }

        public DateTimeOffset Now { get; set; }

        public int Version { get; set; }

        public long Capabilities { get; set; }
    }
}
