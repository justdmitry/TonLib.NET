using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("unpackAccountAddress account_address:string = UnpackedAccountAddress")]
    public class UnpackAccountAddress : RequestBase<UnpackedAccountAddress>
    {
        public UnpackAccountAddress(string accountAddress)
        {
            // Proof: https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L1851
            IsStatic = true;

            AccountAddress = accountAddress;
        }

        public string AccountAddress { get; set; }
    }
}
