namespace TonLibDotNet.Requests.Query
{
    [TLSchema("query.estimateFees id:int53 ignore_chksig:Bool = query.Fees")]
    public class EstimateFees : RequestBase<Types.Query.Fees>
    {
        public long Id { get; set; }

        public bool IgnoreChksig { get; set; }
    }
}
