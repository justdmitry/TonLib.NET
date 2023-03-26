using TonLibDotNet.Types.Tvm;

namespace TonLibDotNet.Requests.Smc
{
    [TLSchema("smc.getState id:int53 = tvm.Cell")]
    public class GetState : RequestBase<Cell>
    {
        public GetState(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
