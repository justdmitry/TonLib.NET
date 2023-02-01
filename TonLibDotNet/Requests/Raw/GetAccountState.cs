using System.Text.Json.Serialization;
using TonLibDotNet.Types;

namespace TonLibDotNet.Requests.Raw
{
    [TLSchema("raw.getAccountState account_address:accountAddress = raw.FullAccountState")]
    public class GetAccountState : RequestBase<Types.Raw.FullAccountState>
    {
        public GetAccountState(string accountAddress)
            : this(new AccountAddress(accountAddress))
        {
            // Nothing
        }

        [JsonConstructor]
        public GetAccountState(AccountAddress accountAddress)
        {
            AccountAddress = accountAddress;
        }

        public AccountAddress AccountAddress { get; set; }
    }
}
