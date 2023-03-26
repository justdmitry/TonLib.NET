using TonLibDotNet.Types;

namespace TonLibDotNet.Requests.Smc
{
    [TLSchema("smc.forget id:int53 = Ok")]
    public class Forget : RequestBase<Ok>
    {
        public Forget(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
