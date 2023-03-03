using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("packAccountAddress account_address:unpackedAccountAddress = AccountAddress")]
    public class PackAccountAddress : RequestBase<AccountAddress>
    {
        public PackAccountAddress(UnpackedAccountAddress accountAddress)
        {
            // Proof: https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L1850
            IsStatic = true;

            AccountAddress = accountAddress ?? throw new ArgumentNullException(nameof(accountAddress));
        }

        public UnpackedAccountAddress AccountAddress { get; set; }
    }
}
