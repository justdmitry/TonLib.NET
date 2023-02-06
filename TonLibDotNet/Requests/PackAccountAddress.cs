using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("packAccountAddress account_address:unpackedAccountAddress = AccountAddress")]
    public class PackAccountAddress : RequestBase<AccountAddress>
    {
        public PackAccountAddress(UnpackedAccountAddress accountAddress)
        {
            AccountAddress = accountAddress ?? throw new ArgumentNullException(nameof(accountAddress));
        }

        public UnpackedAccountAddress AccountAddress { get; set; }
    }
}
