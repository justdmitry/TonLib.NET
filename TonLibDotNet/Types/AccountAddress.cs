using System.Text.Json.Serialization;

namespace TonLibDotNet.Types
{
    [TLSchema("accountAddress account_address:string = AccountAddress")]
    public class AccountAddress : TypeBase
    {
        public AccountAddress(string value)
        {
            Value = value;
        }

        [JsonPropertyName("account_address")]
        public string Value { get; set; }
    }
}
