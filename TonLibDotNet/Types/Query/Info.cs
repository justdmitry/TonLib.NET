namespace TonLibDotNet.Types.Query
{
    [TLSchema("query.info id:int53 valid_until:int53 body_hash:bytes body:bytes init_state:bytes = query.Info")]
    public class Info : TypeBase
    {
        public long Id { get; set; }

        public long ValidUntil { get; set; }

        public string BodyHash { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public string InitState { get; set; } = string.Empty;
    }
}
