using System.Text.Json.Serialization;

namespace TonLibDotNet.Requests
{
    [TLSchema("getAccountState account_address:accountAddress = FullAccountState")]
    public class GetAccountState : RequestBase<FullAccountState>
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
