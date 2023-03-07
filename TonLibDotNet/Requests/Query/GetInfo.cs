using TonLibDotNet.Types.Query;

namespace TonLibDotNet.Requests.Query
{
    [TLSchema("query.getInfo id:int53 = query.Info")]
    public class GetInfo: RequestBase<Info>
    {
        public GetInfo(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
