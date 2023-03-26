using TonLibDotNet.Types.Tvm;

namespace TonLibDotNet.Requests.Smc
{
    [TLSchema("smc.getCode id:int53 = tvm.Cell")]
    public class GetCode : RequestBase<Cell>
    {
        public GetCode(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
