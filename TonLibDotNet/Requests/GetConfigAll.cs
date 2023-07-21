using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("getConfigAll mode:# = ConfigInfo")]
    public class GetConfigAll : RequestBase<ConfigInfo>
    {
        public int Mode { get; set; }
    }
}
