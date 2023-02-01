using TonLibDotNet.Types;

namespace TonLibDotNet.Requests.Raw
{
    [TLSchema("raw.getAccountState account_address:accountAddress = raw.FullAccountState")]
    public class GetAccountState : RequestBase<Types.Raw.FullAccountState>
    {
        public GetAccountState(AccountAddress accountAddress)
        {
            AccountAddress = accountAddress;
        }

        public AccountAddress AccountAddress { get; set; }
    }
}
