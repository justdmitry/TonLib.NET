using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("unpackAccountAddress account_address:string = UnpackedAccountAddress")]
    public class UnpackAccountAddress : RequestBase<UnpackedAccountAddress>
    {
        public UnpackAccountAddress(string accountAddress)
        {
            IsStatic = true;
            AccountAddress = accountAddress;
        }

        public string AccountAddress { get; set; }
    }
}
