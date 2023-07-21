using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("getConfigParam mode:# param:# = ConfigInfo")]
    public class GetConfigParam : RequestBase<ConfigInfo>
    {
        public int Mode { get; set; }

        public int Param { get; set; }
    }
}
