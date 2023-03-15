using TonLibDotNet.Types;
using TonLibDotNet.Types.Dns;

namespace TonLibDotNet.Requests.Dns
{
    [TLSchema("dns.resolve account_address:accountAddress name:string category:int256 ttl:int32 = dns.Resolved")]
    public class Resolve : RequestBase<Resolved>
    {
        public string Name { get; set; } = string.Empty;

        public AccountAddress? AccountAddress { get; set; }

        public string? Category { get; set; }

        public int? Ttl { get; set; }
    }
}
