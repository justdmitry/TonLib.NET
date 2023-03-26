using TonLibDotNet.Types.Tvm;

namespace TonLibDotNet.Requests.Smc
{
    [TLSchema("smc.getData id:int53 = tvm.Cell")]
    public class GetData : RequestBase<Cell>
    {
        public GetData(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
