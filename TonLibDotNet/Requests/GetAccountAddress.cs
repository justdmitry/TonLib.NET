using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("getAccountAddress initial_account_state:InitialAccountState revision:int32 workchain_id:int32 = AccountAddress")]
    public class GetAccountAddress : RequestBase<AccountAddress>
    {
        public GetAccountAddress(InitialAccountState initialAccountState)
        {
            // Proof: https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L1962
            IsStatic = true;

            InitialAccountState = initialAccountState ?? throw new ArgumentNullException(nameof(initialAccountState));
        }

        public InitialAccountState InitialAccountState { get; set; }

        public int Revision { get; set; }

        public int WorkchainId { get; set; }
    }
}
