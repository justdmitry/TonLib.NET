namespace TonLibDotNet.Types.Raw
{
    [TLSchema("raw.accountState code:bytes data:bytes frozen_hash:bytes = AccountState")]
    public class AccountState : Types.AccountState
    {
        public string Code { get; set; } = string.Empty;

        public string Data { get; set; } = string.Empty;

        public string FrozenHash { get; set; } = string.Empty;
    }
}
