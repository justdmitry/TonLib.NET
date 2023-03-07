using TonLibDotNet.Types;

namespace TonLibDotNet.Requests.Query
{
    [TLSchema("query.send id:int53 = Ok")]
    public class Send : RequestBase<Ok>
    {
        public Send(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
